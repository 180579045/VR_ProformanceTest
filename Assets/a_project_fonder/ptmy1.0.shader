Shader "grb/ptmy1.0" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags{"Queue" = "Transparent"}
		GrabPass{}
		Pass{
			SetTexture[_MainTex]{combine texture}
			SetTexture[_GrabTexture]{combine texture*previous}
		}
	
	} 
	FallBack "Diffuse"
}
