Shader "H3D/InGame/SceneStaticObjects/SimpleDiffuse ( no Supports Lightmap)" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	Pass {
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] {
			constantColor (1,1,1,1)
			combine texture, constant // UNITY_OPAQUE_ALPHA_FFP
	    } 
	 }//end pass
	 UsePass "Hidden/H3D ShadowCaster/SHADOWCASTER"
  }  //end subshader
  
}

