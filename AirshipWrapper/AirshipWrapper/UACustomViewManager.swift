/* Copyright Airship and Contributors */

import Foundation
import UIKit
import SwiftUI
import AirshipCore
import AirshipScenes

/// ObjC-friendly mirror of `AirshipCustomViewArguments.SizeInfo`.
@objc(UACustomViewSizeInfo)
public final class UACustomViewSizeInfo: NSObject {
    @objc public let isAutoHeight: Bool
    @objc public let isAutoWidth: Bool

    init(_ sizeInfo: AirshipCustomViewArguments.SizeInfo) {
        self.isAutoHeight = sizeInfo.isAutoHeight
        self.isAutoWidth = sizeInfo.isAutoWidth
    }
}

/// ObjC-friendly mirror of `AirshipCustomViewArguments`.
///
/// `properties` is exposed as a JSON string rather than `AirshipJSON`, which has no
/// ObjC representation. It is `nil` when the Scene supplied no properties.
@objc(UACustomViewArguments)
public final class UACustomViewArguments: NSObject {
    @objc public let name: String
    @objc public let propertiesJSON: String?
    @objc public let sizeInfo: UACustomViewSizeInfo

    init(_ args: AirshipCustomViewArguments) {
        self.name = args.name
        self.sizeInfo = UACustomViewSizeInfo(args.sizeInfo)

        if let properties = args.properties {
            self.propertiesJSON = try? properties.toString()
        } else {
            self.propertiesJSON = nil
        }
    }
}

/// Wraps a `UIView` produced by managed code so SwiftUI can render it inside a Scene.
///
/// The managed side owns the view's own sizing; we only forward the layout pass.
private struct UACustomViewRepresentable: UIViewRepresentable {
    let view: UIView

    func makeUIView(context: Context) -> UIView { view }
    func updateUIView(_ uiView: UIView, context: Context) {}
}

/// ObjC-friendly wrapper around `AirshipCustomViewManager`.
///
/// `AirshipCustomViewManager` registers a SwiftUI `ViewBuilder`, which cannot cross the
/// ObjC boundary. This shim accepts a block returning a `UIView` and adapts it via
/// `UIViewRepresentable`.
@objc(UACustomViewManager)
@MainActor
public final class UACustomViewManager: NSObject {

    @objc public static let shared = UACustomViewManager()

    /// Registers a custom view builder for `name`.
    ///
    /// The block is invoked on the main actor each time a Scene needs the view, and must
    /// return a non-nil `UIView`. Returning nil renders an empty view.
    @objc(registerWithName:builder:)
    public func register(name: String, builder: @escaping (UACustomViewArguments) -> UIView?) {
        AirshipCustomViewManager.shared.register(name: name) { args in
            guard let view = builder(UACustomViewArguments(args)) else {
                return EmptyView()
            }
            return UACustomViewRepresentable(view: view)
        }
    }

    /// Unregisters the custom view builder for `name`.
    @objc(unregisterWithName:)
    public func unregister(name: String) {
        AirshipCustomViewManager.shared.unregister(name: name)
    }
}
