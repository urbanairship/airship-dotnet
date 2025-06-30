using System;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Airship
{
    /// <summary>
    /// Extensions for UAMessageCenter to work around SDK 19 issues
    /// </summary>
    public partial class UAMessageCenter
    {
        private static bool isInitialized = false;
        private static readonly object initLock = new object();
        
        /// <summary>
        /// Safely initializes the Message Center to avoid crashes
        /// </summary>
        public static void EnsureInitialized()
        {
            lock (initLock)
            {
                if (isInitialized) return;
                
                try
                {
                    // Enable Message Center in Privacy Manager if needed
                    var privacyManager = UAirship.PrivacyManager;
                    if (privacyManager != null && !privacyManager.IsEnabled(UAFeature.MessageCenter))
                    {
                        privacyManager.EnableFeatures(UAFeature.MessageCenter);
                        
                        // Wait a bit for initialization
                        System.Threading.Thread.Sleep(100);
                    }
                    
                    // Access the Message Center to force initialization
                    var messageCenter = UAirship.MessageCenter;
                    if (messageCenter != null)
                    {
                        // Try to access the inbox to ensure it's loaded
                        var _ = messageCenter.Inbox;
                        isInitialized = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UAMessageCenter] Failed to initialize: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Displays the Message Center with initialization check
        /// </summary>
        public void DisplaySafely()
        {
            EnsureInitialized();
            
            // Use the main thread to display
            if (NSThread.IsMain)
            {
                Display();
            }
            else
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() => Display());
            }
        }
        
        /// <summary>
        /// Displays a specific message with initialization check
        /// </summary>
        public void DisplaySafely(string messageID)
        {
            EnsureInitialized();
            
            // Use the main thread to display
            if (NSThread.IsMain)
            {
                DisplayWithMessageID(messageID);
            }
            else
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() => DisplayWithMessageID(messageID));
            }
        }
    }
}