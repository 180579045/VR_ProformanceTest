Shader "pt/grid" {
    Properties {
        _MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
        _Color("Color",color) = (1,1,1,1)
    }
    SubShader {

        Pass {
          
            SetTexture [_MainTex] 
            { 
            	constantColor [_Color]
            	combine texture + constant 
            }
        }
    }
}
