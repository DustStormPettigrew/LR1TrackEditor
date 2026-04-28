namespace LR1TrackEditor
{
    using LibLR1;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    public class MabFrameDefinition
    {
        public string MaterialName { get; set; } = string.Empty;
        public int FrameIndex { get; set; }
    }

    public class MabAnimationDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public int SourceIndex { get; set; }
        public int Speed { get; set; }
        public int LogicalFrameCount { get; set; }
        public List<MabFrameDefinition> SequenceFrames { get; } = new List<MabFrameDefinition>();
        public HashSet<string> ReferencedMaterials { get; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public List<MabFrameDefinition> GetPlaybackFrame(int frameIndex)
        {
            List<MabFrameDefinition> result = new List<MabFrameDefinition>();
            if (this.SequenceFrames.Count == 0)
            {
                return result;
            }

            int logicalFrameCount = Math.Max(this.LogicalFrameCount, 1);
            int normalizedFrame = ((frameIndex % logicalFrameCount) + logicalFrameCount) % logicalFrameCount;
            if ((this.SequenceFrames.Count % logicalFrameCount) == 0)
            {
                int entriesPerFrame = this.SequenceFrames.Count / logicalFrameCount;
                int start = normalizedFrame * entriesPerFrame;
                for (int i = 0; i < entriesPerFrame; i++)
                {
                    result.Add(this.SequenceFrames[start + i]);
                }
                return result;
            }

            result.Add(this.SequenceFrames[Math.Min(normalizedFrame, this.SequenceFrames.Count - 1)]);
            return result;
        }
    }

    public class LoadedMabDefinition
    {
        public string SourcePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<MabAnimationDefinition> Animations { get; } = new List<MabAnimationDefinition>();
    }

    public enum AnimatedObjectType
    {
        StaticModel,
        BdbModel,
        AnimatedModel
    }

    public class AnimatedObjectEntry
    {
        public string Id { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string SceneName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string SourcePath { get; set; } = string.Empty;
        public string SceneKey { get; set; } = string.Empty;
        public AnimatedObjectType ObjectType { get; set; }
        public Matrix WorldMatrix { get; set; } = Matrix.Identity;
        public WDB Scene { get; set; }
        public List<string> MaterialNames { get; } = new List<string>();
        public List<MabAnimationDefinition> Animations { get; } = new List<MabAnimationDefinition>();

        public override string ToString()
        {
            string scenePart = string.IsNullOrWhiteSpace(this.SceneName) ? "Scene" : this.SceneName;
            string modelPart = string.IsNullOrWhiteSpace(this.ModelName) ? "No model" : this.ModelName;
            return scenePart + " :: " + this.ObjectName + " [" + modelPart + "]";
        }
    }

    public class AnimatedObjectPlayback
    {
        public AnimatedObjectEntry Entry { get; set; }
        public MabAnimationDefinition Animation { get; set; }
        public bool Loop { get; set; }
        public float ElapsedSeconds { get; set; }

        public int GetCurrentFrameIndex()
        {
            if (this.Animation == null)
            {
                return 0;
            }

            int logicalFrameCount = Math.Max(this.Animation.LogicalFrameCount, 1);
            float framesPerSecond = Math.Max(this.Animation.Speed, 1);
            int rawFrame = (int)(this.ElapsedSeconds * framesPerSecond);
            if (this.Loop)
            {
                return rawFrame % logicalFrameCount;
            }

            return Math.Min(rawFrame, logicalFrameCount - 1);
        }

        public bool IsFinished()
        {
            if (this.Animation == null || this.Loop)
            {
                return false;
            }

            int logicalFrameCount = Math.Max(this.Animation.LogicalFrameCount, 1);
            float framesPerSecond = Math.Max(this.Animation.Speed, 1);
            return this.ElapsedSeconds * framesPerSecond >= logicalFrameCount;
        }
    }
}
