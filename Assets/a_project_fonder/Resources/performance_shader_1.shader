Shader "pt/Alpha Texst" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }
    SubShader {
       
        Pass {
            AlphaTest Greater 0.2
            SetTexture [_MainTex] { combine texture }
        }
    }
}
