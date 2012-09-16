// ======================================================================================
// File         : SpriteBlendPixelPerfect.shader
// Author       : Wu Jie 
// Last Change  : 08/07/2012 | 10:20:27 AM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

Shader "ex2D/Alpha Blended (Pixel-Perfect Adjust)" {
    Properties {
        _MainTex ("Atlas Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off 
        Lighting Off 
        ZWrite Off 
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

            sampler2D _MainTex;

            // round-to-nearest even profiles
            float round(float a)
            {
                float x = a + 0.5;
                float f = floor(x);
                float r;
                if (x == f) {
                    if (a > 0)
                        r = f - fmod(f, 2);
                    else
                        r = f + fmod(f, 2);
                } else
                    r = f;
                return r;
            }

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
				float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MV, v.vertex);
                // o.vertex.xy += float2(0.5,0.5);
                // o.vertex.x = round(o.vertex.x+0.5);
                // o.vertex.y = round(o.vertex.y+0.5);
                o.vertex = mul(UNITY_MATRIX_P, o.vertex);

                o.color = v.color;
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) :COLOR {
                return tex2D(_MainTex, i.texcoord) * i.color;
            }
            ENDCG
        }
    }
    Fallback off
}
