Shader "pt/diffuse" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
    }
    SubShader {
       
        Pass {
            Material{
               Diffuse(1,1,1,1)
               Ambient(1,1,1,1)
            }
            Lighting ON
            SetTexture [_MainTex] { combine texture*primary DOUBLE }
        }
    }
}

