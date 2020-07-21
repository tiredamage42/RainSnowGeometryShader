Shader "Hidden/Environment/Rain" {
	Properties { }
    SubShader{
        Tags{ 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True" 
        }
        CULL OFF
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass {
            CGPROGRAM
            #pragma multi_compile_instancing        
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma target 4.0
            #define RAIN
            #include "Precipitation.cginc"       
            ENDCG
        }
    }
}