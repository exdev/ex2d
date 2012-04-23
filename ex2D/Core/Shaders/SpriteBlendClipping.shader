// ======================================================================================
// File         : SpriteBlendClipping.shader
// Author       : Wu Jie 
// Last Change  : 03/05/2012 | 15:19:25 PM | Monday,March
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

Shader "ex2D/Alpha Blended (Clipping)" {
    Properties {
        _MainTex ("Atlas Texture", 2D) = "white" {}
    }

    // ======================================================== 
    // cg 
    // ======================================================== 

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off 
        Lighting Off 
        ZWrite Off 
        Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _ClipRect = float4(0.0, 0.0, 1.0, 1.0);

			struct appdata_t {
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex        : POSITION;
				fixed4 color         : COLOR;
				float2 texcoord      : TEXCOORD0;
				float2 worldPosition : TEXCOORD1;
			};

			v2f vert ( appdata_t _v ) {
				v2f o;
				o.worldPosition = mul(_Object2World, _v.vertex).xy;
				o.vertex = mul(UNITY_MATRIX_MVP, _v.vertex);
				o.color = _v.color;
				o.texcoord = TRANSFORM_TEX(_v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag ( v2f _in ) : COLOR {
                _ClipRect = float4( 0.0, 0.0, 20.0, 40.0 ); // DELME TEMP
				float2 factor = abs ( _in.worldPosition - _ClipRect.xy ) / _ClipRect.zw;
				clip ( 1.0 - max ( factor.x, factor.y ) );
				return tex2D ( _MainTex, _in.texcoord ) * _in.color;
			}
			ENDCG
        }
    }

    // ======================================================== 
    // fallback 
    // ======================================================== 

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
        }
        Cull Off 
        Lighting Off 
        ZWrite Off 
        Fog { Color (0,0,0,0) }
        Blend SrcAlpha OneMinusSrcAlpha

        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
        }

        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
    }
}
