using UnityEngine;

public static class CustomColor
{
        public static Color HotBlood
    {
        get
        {
                   ColorUtility.TryParseHtmlString("#ff0050", out Color result);
        return result; 
        }

    }
        public static Color BadBlood
    {
                get
        {
        ColorUtility.TryParseHtmlString("#6400ff", out Color result);
        return result;
    }}
        public static Color OldBlood=>Color.Lerp(HotBlood,Barrier,0.8f);

        public static Color White
    {
                get
        {
        ColorUtility.TryParseHtmlString("#ffffff", out Color result);
        return result;}
    }

     public static Color Barrier
    {
                get
        {
        ColorUtility.TryParseHtmlString("#140033", out Color result);
        return result;
        }
    }
            public static Color GroundDark
    {
                get
        {
        ColorUtility.TryParseHtmlString("#676273", out Color result);
        return result;
        }
    }
                public static Color GroundGlow
    {        get
        {
        ColorUtility.TryParseHtmlString("#a298b3", out Color result);
        return result;
        }
    }
}