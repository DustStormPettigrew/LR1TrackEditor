namespace LR1TrackEditor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) Synchronized(new Settings()));

        public static Settings Default =>
            defaultInstance;

        [DebuggerNonUserCode, DefaultSettingValue("True"), UserScopedSetting]
        public bool doTextures
        {
            get => 
                (bool) this["doTextures"];
            set => 
                this["doTextures"] = value;
        }

        [DefaultSettingValue("60"), DebuggerNonUserCode, UserScopedSetting]
        public float FoV
        {
            get => 
                (float) this["FoV"];
            set => 
                this["FoV"] = value;
        }

        [DefaultSettingValue("1000"), DebuggerNonUserCode, UserScopedSetting]
        public float RenderDistance
        {
            get => 
                (float) this["RenderDistance"];
            set => 
                this["RenderDistance"] = value;
        }

        [DefaultSettingValue("2"), DebuggerNonUserCode, UserScopedSetting]
        public float FlySpeed
        {
            get => 
                (float) this["FlySpeed"];
            set => 
                this["FlySpeed"] = value;
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool doVertexColors
        {
            get => 
                (bool) this["doVertexColors"];
            set => 
                this["doVertexColors"] = value;
        }

        [UserScopedSetting, DefaultSettingValue("CornflowerBlue"), DebuggerNonUserCode]
        public Color BackgroundColor
        {
            get => 
                (Color) this["BackgroundColor"];
            set => 
                this["BackgroundColor"] = value;
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool doSkybox
        {
            get => 
                (bool) this["doSkybox"];
            set => 
                this["doSkybox"] = value;
        }

        [DefaultSettingValue("False"), DebuggerNonUserCode, UserScopedSetting]
        public bool AutoloadPowerup
        {
            get => 
                (bool) this["AutoloadPowerup"];
            set => 
                this["AutoloadPowerup"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("False")]
        public bool AutoloadObject
        {
            get => 
                (bool) this["AutoloadObject"];
            set => 
                this["AutoloadObject"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadRacerPaths
        {
            get =>
                (bool)this["TrackLoadRacerPaths"];
            set =>
                this["TrackLoadRacerPaths"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadPowerups
        {
            get =>
                (bool)this["TrackLoadPowerups"];
            set =>
                this["TrackLoadPowerups"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadSkybox
        {
            get =>
                (bool)this["TrackLoadSkybox"];
            set =>
                this["TrackLoadSkybox"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadStartPositions
        {
            get =>
                (bool)this["TrackLoadStartPositions"];
            set =>
                this["TrackLoadStartPositions"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadCheckpoints
        {
            get =>
                (bool)this["TrackLoadCheckpoints"];
            set =>
                this["TrackLoadCheckpoints"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadHazards
        {
            get =>
                (bool)this["TrackLoadHazards"];
            set =>
                this["TrackLoadHazards"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadEmitters
        {
            get =>
                (bool)this["TrackLoadEmitters"];
            set =>
                this["TrackLoadEmitters"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool TrackLoadCollisionGeometry
        {
            get =>
                (bool)this["TrackLoadCollisionGeometry"];
            set =>
                this["TrackLoadCollisionGeometry"] = value;
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool ShowConsole
        {
            get => 
                (bool) this["ShowConsole"];
            set => 
                this["ShowConsole"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool GhostPlacing
        {
            get => 
                (bool) this["GhostPlacing"];
            set => 
                this["GhostPlacing"] = value;
        }

        [DebuggerNonUserCode, UserScopedSetting, DefaultSettingValue("True")]
        public bool NeedsUpdate
        {
            get => 
                (bool) this["NeedsUpdate"];
            set => 
                this["NeedsUpdate"] = value;
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("1084, 637")]
        public Size FormSize
        {
            get => 
                (Size) this["FormSize"];
            set => 
                this["FormSize"] = value;
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting]
        public string Setting
        {
            get => 
                (string) this["Setting"];
            set => 
                this["Setting"] = value;
        }
    }
}

