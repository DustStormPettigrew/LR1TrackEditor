namespace LR1TrackEditor
{
    using LibLR1;
    using System.Collections.Generic;

    public class LoadedEmitterDefinition
    {
        public string SourcePath { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public EMB Source { get; set; }
        public Dictionary<string, Material> Materials { get; } = new Dictionary<string, Material>(System.StringComparer.InvariantCultureIgnoreCase);
        public List<LoadedMabDefinition> MaterialAnimations { get; } = new List<LoadedMabDefinition>();
    }
}
