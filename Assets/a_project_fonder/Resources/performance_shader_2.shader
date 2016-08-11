Shader "pt/Blending" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }
    SubShader {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha 
        Pass {
            SetTexture [_MainTex] { combine texture }
        }
    }
}