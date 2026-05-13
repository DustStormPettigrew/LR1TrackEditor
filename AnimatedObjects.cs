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
            bool hasExplicitTimelineKeys =
                this.SequenceFrames.Count > 1 &&
                this.SequenceFrames.TrueForAll(frame => frame != null) &&
                this.SequenceFrames.Exists(frame => frame.FrameIndex > 0) &&
                this.SequenceFrames.TrueForAll(frame => frame.FrameIndex >= 0 && frame.FrameIndex < logicalFrameCount);
            if (hasExplicitTimelineKeys)
            {
                MabFrameDefinition selectedFrame = this.SequenceFrames[0];
                foreach (MabFrameDefinition frame in this.SequenceFrames)
                {
                    if (frame.FrameIndex > normalizedFrame)
                    {
                        break;
                    }

                    selectedFrame = frame;
                }

                result.Add(selectedFrame);
                return result;
            }

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

    public class LoadedAdbDefinition
    {
        public string SourcePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ADB Source { get; set; }
        public List<AdbAnimationDefinition> Animations { get; } = new List<AdbAnimationDefinition>();
    }

    public class LoadedSdbDefinition
    {
        public string SourcePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public SDB Source { get; set; }
    }

    public class AdbAnimationDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public string AnimationName { get; set; } = string.Empty;
        public int SourceIndex { get; set; }
        public ADB_Meta Meta { get; set; }
        public LoadedAdbDefinition SourceDefinition { get; set; }
    }

    public enum AnimatedObjectAnimationKind
    {
        Material,
        Transform
    }

    public class AnimatedObjectAnimationOption
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public AnimatedObjectAnimationKind Kind { get; set; }
        public MabAnimationDefinition MaterialAnimation { get; set; }
        public AdbAnimationDefinition TransformAnimation { get; set; }

        public int LogicalFrameCount
        {
            get
            {
                if (this.Kind == AnimatedObjectAnimationKind.Material)
                {
                    return Math.Max(this.MaterialAnimation?.LogicalFrameCount ?? 0, 1);
                }

                return Math.Max(this.TransformAnimation?.Meta?.Length ?? 0, 1);
            }
        }

        public int Speed
        {
            get
            {
                if (this.Kind == AnimatedObjectAnimationKind.Material)
                {
                    return Math.Max(this.MaterialAnimation?.Speed ?? 0, 1);
                }

                return Math.Max(this.TransformAnimation?.Meta?.Speed ?? 0, 1);
            }
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
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
        public int? AdbIndex { get; set; }
        public int? SdbIndex { get; set; }
        public LoadedAdbDefinition AdbDefinition { get; set; }
        public LoadedSdbDefinition SdbDefinition { get; set; }
        public List<string> MaterialNames { get; } = new List<string>();
        public List<MabAnimationDefinition> Animations { get; } = new List<MabAnimationDefinition>();
        public List<AnimatedObjectAnimationOption> AvailableAnimations { get; } = new List<AnimatedObjectAnimationOption>();

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
        public AnimatedObjectAnimationOption AnimationOption { get; set; }
        public bool Loop { get; set; }
        public float ElapsedSeconds { get; set; }

        public MabAnimationDefinition MaterialAnimation =>
            this.AnimationOption?.MaterialAnimation;

        public AdbAnimationDefinition TransformAnimation =>
            this.AnimationOption?.TransformAnimation;

        public float GetCurrentFrameTime()
        {
            if (this.AnimationOption == null)
            {
                return 0f;
            }

            int logicalFrameCount = Math.Max(this.AnimationOption.LogicalFrameCount, 1);
            float framesPerSecond = Math.Max(this.AnimationOption.Speed, 1);
            float rawFrame = this.ElapsedSeconds * framesPerSecond;
            if (this.Loop)
            {
                return rawFrame % logicalFrameCount;
            }

            return Math.Min(rawFrame, logicalFrameCount - 1);
        }

        public int GetCurrentFrameIndex()
        {
            return (int)this.GetCurrentFrameTime();
        }

        public bool IsFinished()
        {
            if (this.AnimationOption == null || this.Loop)
            {
                return false;
            }

            int logicalFrameCount = Math.Max(this.AnimationOption.LogicalFrameCount, 1);
            float framesPerSecond = Math.Max(this.AnimationOption.Speed, 1);
            return this.ElapsedSeconds * framesPerSecond >= logicalFrameCount;
        }
    }
}
