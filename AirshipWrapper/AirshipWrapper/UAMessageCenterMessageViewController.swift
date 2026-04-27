/* Copyright Airship and Contributors */

import Foundation
import UIKit
import SwiftUI
import AirshipMessageCenter

/// A view controller that displays a single Message Center message.
@objc(UAMessageCenterMessageViewController)
@MainActor
public final class UAMessageCenterMessageViewController: UIViewController {

    private let messageID: String
    private var hostingController: UIViewController?

    /// Called after the message view has been set up and had one run-loop pass to render.
    @objc public var onViewReady: (() -> Void)?

    @objc
    public init(messageID: String) {
        self.messageID = messageID
        super.init(nibName: nil, bundle: nil)
    }

    @available(*, unavailable)
    required init?(coder: NSCoder) {
        fatalError("init(coder:) is not supported")
    }

    public override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = .systemBackground

        let messageView = MessageCenterMessageView(messageID: messageID)
        let host = UIHostingController(rootView: messageView)
        host.view.backgroundColor = .clear
        host.view.translatesAutoresizingMaskIntoConstraints = false

        addChild(host)
        view.addSubview(host.view)

        NSLayoutConstraint.activate([
            host.view.topAnchor.constraint(equalTo: view.topAnchor),
            host.view.leadingAnchor.constraint(equalTo: view.leadingAnchor),
            host.view.trailingAnchor.constraint(equalTo: view.trailingAnchor),
            host.view.bottomAnchor.constraint(equalTo: view.bottomAnchor),
        ])

        host.didMove(toParent: self)
        hostingController = host

        Task { @MainActor in self.onViewReady?() }
    }
}
