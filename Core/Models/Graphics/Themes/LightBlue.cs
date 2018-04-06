using FrostAura.Libraries.Core.Interfaces.Graphics;

namespace FrostAura.Libraries.Core.Models.Graphics.Themes
{
    /// <summary>
    /// Light blue theme parameters.
    /// </summary>
    public class LightBlue : ITheme
    {
        public string Identifier => "LightBlue";
        
        public RGB Primary => new RGB { R = 0, G = 125, B = 255 };
        public string PrimaryHex => "#007dff";
        
        public RGB PrimaryDark => new RGB { R = 0, G = 60, B = 125 };
        public string PrimaryDarkHex => "#003c7d";
        
        public RGB PrimaryLight => new RGB { R = 0, G = 220, B = 255 };
        public string PrimaryLightHex => "#00dcff";
        
        public RGB TextAndIcons => new RGB { R = 255, G = 255, B = 255 };
        public string TextAndIconsHex => "#FFFFFF";
        
        public RGB Accent => new RGB { R = 55, G = 55, B = 55 };
        public string AccentHex => "#373737";
        
        public RGB PrimaryText => new RGB { R = 25, G = 25, B = 25 };
        public string PrimaryTextHex => "#191919";
        
        public RGB SecondaryText => new RGB { R = 80, G = 80, B = 80 };
        public string SecondaryTextHex => "#505050";
    }
}