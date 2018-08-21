Shader "Custom/MultiTexturing" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SecondTex("SecondTex", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecondTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SecondTex;
			float4 color : COlOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			fixed4 mainTex = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 secondTex = tex2D(_SecondTex, IN.uv_SecondTex) * _Color;

			o.Albedo = lerp(secondTex.rgb, mainTex.rgb, IN.color.r);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = mainTex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
