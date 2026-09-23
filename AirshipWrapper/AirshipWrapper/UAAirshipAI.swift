/* Copyright Airship and Contributors */

import Foundation
import AirshipCore
import AirshipAutomation

/// ObjC-friendly mirror of `AirshipAI.Context.Item`.
@objc(UAEvaluationContextItem)
public final class UAEvaluationContextItem: NSObject {
    /// Self-describing context text, inserted into the prompt as-is.
    @objc public let content: String

    /// Relative importance, where **lower is more important**. Items with the highest
    /// value are dropped first when the prompt exceeds the model's input window.
    @objc public let priority: Double

    @objc public init(content: String, priority: Double) {
        self.content = content
        self.priority = priority
    }

    var asItem: AirshipAI.Context.Item {
        AirshipAI.Context.Item(content: content, priority: priority)
    }
}

/// ObjC-friendly wrapper around `Airship.ai`'s context provider registration.
///
/// The native API is generic over a per-usage `Subject` type and takes an `async` closure,
/// neither of which crosses the ObjC boundary. This shim keys usages by their stable raw
/// string and hands the subject across as JSON, so managed code can register a provider for
/// any usage without the SDK's phantom types.
///
/// Only context provision is exposed. Supplying a custom `ModelAdapter` is not — it requires
/// generation schemas and structured output that have no useful ObjC representation.
@objc(UAAirshipAI)
@MainActor
public final class UAAirshipAI: NSObject {

    @objc public static let shared = UAAirshipAI()

    /// Raw value of `AirshipAI.InAppMessageSuppression.usage`.
    @objc public static let usageInAppMessageSuppression = "in_app_message_suppression"

    /// Registers a context provider for `usage`.
    ///
    /// The block receives the subject encoded as a JSON string (or nil when the usage has
    /// no subject payload) and returns the context items to contribute. Pass a nil block
    /// to clear the provider.
    ///
    /// - Returns: `true` if the usage is recognized and the provider was applied.
    @objc(setContextProviderForUsage:provider:)
    @discardableResult
    public func setContextProvider(
        usage: String,
        provider: ((String?) -> [UAEvaluationContextItem])?
    ) -> Bool {
        switch usage {
        case Self.usageInAppMessageSuppression:
            guard let provider else {
                Airship.ai.setContextProvider(for: AirshipAI.InAppMessageSuppression.usage, nil)
                return true
            }

            Airship.ai.setContextProvider(for: AirshipAI.InAppMessageSuppression.usage) { subject in
                let json = Self.encodeSuppressionSubject(subject)
                let items = await MainActor.run { provider(json) }
                return AirshipAI.Context(items: items.map { $0.asItem })
            }
            return true

        default:
            return false
        }
    }

    /// Registers the fallback context provider used for usages with no provider of their own.
    ///
    /// A usage-specific provider wins outright; the two are never combined. Pass a nil block
    /// to clear it.
    @objc(setDefaultContextProvider:)
    public func setDefaultContextProvider(_ provider: (() -> [UAEvaluationContextItem])?) {
        guard let provider else {
            Airship.ai.setDefaultContextProvider(nil)
            return
        }

        Airship.ai.setDefaultContextProvider {
            let items = await MainActor.run { provider() }
            return AirshipAI.Context(items: items.map { $0.asItem })
        }
    }

    /// Encodes an in-app suppression subject so managed code can inspect it without the
    /// Swift type. Keys match the Android subject's field names so both platforms agree.
    private nonisolated static func encodeSuppressionSubject(
        _ subject: AirshipAI.InAppMessageSuppression.Subject
    ) -> String? {
        var payload: [String: Any] = [
            "name": subject.name,
            "priority": subject.priority,
            "hints": subject.hints
        ]

        if let extras = subject.extras, let string = try? extras.toString() {
            payload["extras"] = string
        }

        guard
            JSONSerialization.isValidJSONObject(payload),
            let data = try? JSONSerialization.data(withJSONObject: payload)
        else {
            return nil
        }

        return String(data: data, encoding: .utf8)
    }
}
