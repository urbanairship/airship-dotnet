﻿/* Copyright Airship and Contributors */

namespace AirshipDotNet.Channel
{
    /// <summary>
    /// Editor for Channel and Named User tag groups.
    /// </summary>
    public class TagGroupsEditor
    {
        private readonly List<TagOperation> operations = new();
        private readonly Action<List<TagOperation>> onApply;

        //@cond IGNORE
        public TagGroupsEditor(Action<List<TagOperation>> onApply)
        {
            this.onApply = onApply;
        }
        //@endcond

        /// <summary>
        /// Add a tag to a given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tag">Tag to add.</param>
        /// <param name="group">Group to add the tag to.</param>
        public TagGroupsEditor AddTag(string tag, string group)
        {
            return AddTags(new string[] { tag }, group);
        }

        /// <summary>
        /// Add tags to a given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tags">Tags to add.</param>
        /// <param name="group">Group to add tags to.</param>
        public TagGroupsEditor AddTags(ICollection<string> tags, string group)
        {
            operations.Add(new TagOperation(OperationType.ADD, tags, group));
            return this;

        }

        /// <summary>
        /// Remove tag from a given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tag">Tag to remove.</param>
        /// <param name="group">Group to remove the tag from.</param>
        public TagGroupsEditor RemoveTag(string tag, string group)
        {
            return RemoveTags(new string[] { tag }, group);
        }

        /// <summary>
        /// Remove tags from a given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tags">Tags to remove.</param>
        /// <param name="group">Group to remove the tags from.</param>
        public TagGroupsEditor RemoveTags(ICollection<string> tags, string group)
        {
            operations.Add(new TagOperation(OperationType.REMOVE, tags, group));
            return this;
        }

        /// <summary>
        /// Set a tag to the given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tag">Tag to set.</param>
        /// <param name="group">Group to set the tag to.</param>
        public TagGroupsEditor SetTag(string tag, string group)
        {
            return SetTags(new string[] { tag }, group);
        }

        /// <summary>
        /// Set tags to the given tag group.
        /// </summary>
        /// <returns>The tag groups editor.</returns>
        /// <param name="tags">Tags to set.</param>
        /// <param name="group">Group to set the tags to.</param>
        public TagGroupsEditor SetTags(ICollection<string> tags, string group)
        {
            operations.Add(new TagOperation(OperationType.SET, tags, group));
            return this;
        }

        /// <summary>
        /// Apply the tag group changes.
        /// </summary>
        public void Apply()
        {
            onApply?.Invoke(operations);
        }

        //@cond IGNORE
        public class TagOperation
        {
            public OperationType operationType;
            public ICollection<string> tags;
            public string group;

            internal TagOperation(OperationType operation, ICollection<string> tags, string group)
            {
                operationType = operation;
                this.tags = tags;
                this.group = group;
            }
        }

        public enum OperationType { ADD, REMOVE, SET }
        //@endcond
    }
}
