Shader "pt/ Texture" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }
    SubShader {
        blend off
        Pass {
            SetTexture [_MainTex] { combine texture }
        }
    }
}
