
Shader "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"  {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
    LOD 100
	Tags { "RenderType"="Opaque" }
	Pass {
		Material {
			Diffuse (1,1,1,1)
			Ambient (1,1,1,1)
		} 
		Lighting On
		Tags { "LightMode" = "Vertex" }
		SetTexture [_MainTex] {
			
			combine texture*primary
		}  
	}
    UsePass "Hidden/H3D ShadowCaster/SHADOWCASTER"
  }   
}