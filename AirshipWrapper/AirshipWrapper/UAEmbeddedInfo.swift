/* Copyright Airship and Contributors */

import Foundation
import AirshipCore

/// ObjC-friendly mirror of AirshipEmbeddedInfo.
@objc(UAEmbeddedInfo)
public final class UAEmbeddedInfo: NSObject {
    @objc public let embeddedID: String
    @objc public let instanceID: String
    @objc public let priority: Int

    init(_ info: AirshipEmbeddedInfo) {
        self.embeddedID = info.embeddedID
        self.instanceID = info.instanceID
        self.priority = info.priority
    }
}
