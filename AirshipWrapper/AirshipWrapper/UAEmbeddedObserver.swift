/* Copyright Airship and Contributors */

import Foundation
import Combine
import AirshipCore

/// ObjC-friendly wrapper around AirshipEmbeddedObserver that reports all pending embedded content.
@objc(UAEmbeddedObserver)
@MainActor
public final class UAEmbeddedObserver: NSObject {

    private let observer = AirshipEmbeddedObserver()
    private var cancellable: AnyCancellable?

    /// Called on the main actor whenever the pending set changes.
    @objc public var onUpdate: (([UAEmbeddedInfo]) -> Void)?

    /// Current snapshot of pending embedded content.
    @objc public private(set) var infos: [UAEmbeddedInfo] = []

    @objc
    public func start() {
        cancellable = observer.$embeddedInfos
            .receive(on: DispatchQueue.main)
            .sink { [weak self] infos in
                guard let self else { return }
                let mapped = infos.map { UAEmbeddedInfo($0) }
                self.infos = mapped
                self.onUpdate?(mapped)
            }
    }

    @objc
    public func stop() {
        cancellable?.cancel()
        cancellable = nil
    }
}
