Shader "Custom/LineIndicator" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		_Displacement ("Displacement", float) = 1.5
	}
 
    SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Off ZWrite Off ZTest Always Fog { Mode Off }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
          float4 screenPos;
      };
      sampler2D _MainTex;
      sampler2D _Detail;
      float _Displacement;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          screenUV *= float2(8,6);
          float dis = _Displacement * 1129.0f;
          o.Albedo = tex2D (_MainTex, screenUV + float2(_Displacement, dis)) * 5;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }