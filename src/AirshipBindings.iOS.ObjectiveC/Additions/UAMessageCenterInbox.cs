using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Airship
{
    /// <summary>
    /// Extensions for UAMessageCenterInbox to work around SDK 19 completion handler issues
    /// </summary>
    /// <remarks>
    /// The Swift async methods in SDK 19 are bridged to Objective-C with a special
    /// runtime encoding that the Xamarin binding generator cannot handle.
    /// This class provides safe alternatives that avoid the problematic methods.
    /// </remarks>
    public partial class UAMessageCenterInbox
    {
        /// <summary>
        /// TEMPORARY: Gets messages safely by returning empty array
        /// </summary>
        /// <remarks>
        /// This is a temporary workaround to prevent crashes. In production, you should:
        /// 1. Use native bindings with proper Swift interop
        /// 2. Create an Objective-C wrapper that exposes simpler APIs
        /// 3. Wait for Xamarin.iOS to support Swift async bridged methods
        /// </remarks>
        public UAMessageCenterMessage[] GetMessagesSafely()
        {
            Console.WriteLine("[UAMessageCenterInbox] WARNING: GetMessagesSafely returning empty array - Swift async bridged methods not supported");
            
            // Return empty array to prevent crash
            // TODO: Implement proper solution using one of these approaches:
            // 1. P/Invoke to objc_msgSend with proper block handling
            // 2. Create Objective-C wrapper framework
            // 3. Use alternative API if available
            
            return new UAMessageCenterMessage[0];
        }
        
        /// <summary>
        /// TEMPORARY: Gets unread count safely by returning 0
        /// </summary>
        public nint GetUnreadCountSafely()
        {
            Console.WriteLine("[UAMessageCenterInbox] WARNING: GetUnreadCountSafely returning 0 - Swift async bridged methods not supported");
            return 0;
        }
        
        /// <summary>
        /// Marks messages as read safely
        /// </summary>
        /// <remarks>
        /// These methods work because they have simpler signatures
        /// </remarks>
        public void MarkReadSafely(string[] messageIDs)
        {
            try
            {
                MarkReadWithMessageIDs(messageIDs, () => { });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UAMessageCenterInbox] MarkReadSafely failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Deletes messages safely
        /// </summary>
        public void DeleteSafely(string[] messageIDs)
        {
            try
            {
                DeleteWithMessageIDs(messageIDs, () => { });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UAMessageCenterInbox] DeleteSafely failed: {ex.Message}");
            }
        }
    }
}