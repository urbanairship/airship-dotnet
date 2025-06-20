using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Airship
{
    public partial class UAMessageCenterInbox
    {
        // Manual binding for getMessagesWithCompletionHandler since it's not being generated
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr block);
        
        delegate void GetMessagesCompletionHandler(IntPtr block, IntPtr messagesArray);
        
        static readonly GetMessagesCompletionHandler GetMessagesTrampoline = TrampolineGetMessages;
        
        [MonoPInvokeCallback(typeof(GetMessagesCompletionHandler))]
        static unsafe void TrampolineGetMessages(IntPtr block, IntPtr messagesArray)
        {
            var descriptor = (BlockLiteral*)block;
            var del = (Action<UAMessageCenterMessage[]>)descriptor->Target;
            if (del != null)
            {
                UAMessageCenterMessage[] messages = null;
                if (messagesArray != IntPtr.Zero)
                {
                    var nsArray = Runtime.GetNSObject<NSArray>(messagesArray);
                    if (nsArray != null)
                    {
                        messages = new UAMessageCenterMessage[nsArray.Count];
                        for (nuint i = 0; i < nsArray.Count; i++)
                        {
                            messages[i] = nsArray.GetItem<UAMessageCenterMessage>(i);
                        }
                    }
                }
                del(messages ?? Array.Empty<UAMessageCenterMessage>());
            }
        }
        
        public unsafe void GetMessages(Action<UAMessageCenterMessage[]> completionHandler)
        {
            if (completionHandler == null)
                throw new ArgumentNullException(nameof(completionHandler));
                
            BlockLiteral block = new BlockLiteral();
            block.SetupBlockUnsafe(GetMessagesTrampoline, completionHandler);
            
            try
            {
                var selector = Selector.GetHandle("getMessagesWithCompletionHandler:");
                void_objc_msgSend_IntPtr(this.Handle, selector, (IntPtr)(&block));
            }
            finally
            {
                block.CleanupBlock();
            }
        }
    }
}