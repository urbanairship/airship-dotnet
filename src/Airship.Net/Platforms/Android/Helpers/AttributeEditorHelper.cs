/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using AirshipDotNet.Attributes;
using Java.Util;

namespace AirshipDotNet.Helpers
{
    /// <summary>
    /// Helper class for applying attribute operations to native Android attribute editors.
    /// </summary>
    internal static class AttributeEditorHelper
    {
        /// <summary>
        /// Applies attribute operations to the given editor.
        /// </summary>
        /// <typeparam name="T">The type of the native attribute editor.</typeparam>
        /// <param name="editor">The native attribute editor.</param>
        /// <param name="operations">The attribute operations to apply.</param>
        public static void ApplyAttributeOperations<T>(T editor, List<AttributeEditor.IAttributeOperation> operations)
            where T : class
        {
            dynamic dynamicEditor = editor;

            foreach (var operation in operations)
            {
                if (operation.OperationType == AttributeEditor.OperationType.REMOVE)
                {
                    dynamicEditor.RemoveAttribute(operation.Key);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<string> stringOp)
                {
                    dynamicEditor.SetAttribute(stringOp.Key, stringOp.Value);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<int> intOp)
                {
                    dynamicEditor.SetAttribute(intOp.Key, intOp.Value);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<long> longOp)
                {
                    dynamicEditor.SetAttribute(longOp.Key, longOp.Value);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<float> floatOp)
                {
                    dynamicEditor.SetAttribute(floatOp.Key, floatOp.Value);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<double> doubleOp)
                {
                    dynamicEditor.SetAttribute(doubleOp.Key, doubleOp.Value);
                }
                else if (operation is AttributeEditor.SetAttributeOperation<DateTime> dateOp)
                {
                    // Convert DateTime to Java Date
                    long epochSeconds = new DateTimeOffset(dateOp.Value).ToUnixTimeSeconds();
                    var date = new Date(epochSeconds * 1000);
                    dynamicEditor.SetAttribute(dateOp.Key, date);
                }
            }
        }
    }
}