Shader "Hidden/Clouds"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _Grid ("Grid", 3D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        // No culling or depth

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float3 _Intensity;
            float3 _LightPos;
            
            float3 _Intensity2;
            float3 _LightPos2;

            float3 _Origin;
            float3 _Dim;
            float3 _SunDir;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler3D _Grid;
            sampler3D _SunBuffer;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            


            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldDir : TEXCOORD1;
                float4 scrPos:TEXCOORD2;
                float4 viewDir : TEXCOORD3;
            };

            float rand31(float3 p){
                p *= float3(21.234,7.31,15.23);
                return frac(sin(length(p)) * 9684.31);
            }

            //value noise
            float noise(float3 p){
                float3 id = floor(p);
                float3 f  = frac(p);
                float2 e = float2(1.,0.);
                float a = rand31(id);
                float b = rand31(id + e.xyy);
                float c = rand31(id + e.yxy);
                float d = rand31(id + e.xxy);
    
                float a2 = rand31(id + e.yyx);
                float b2 = rand31(id + e.xyx);
                float c2 = rand31(id + e.yxx);
                float d2 = rand31(id + e.xxx);
    
                f = f*f *(3. - 2.*f);
    
                return lerp(lerp(lerp(a,b,f.x),
                                 lerp(c,d,f.x),f.y),
                            lerp(lerp(a2,b2,f.x),
                                 lerp(c2,d2,f.x),f.y),f.z);
            }
            
            //perlin fbm
            float pFbm(float3 p){

                p/= 1.25;
                p += _Time[1]*.5;
                float3x3 q = float3x3(4./5., -3./5., 0.,
                                3./5., 4./5.,  0.,
                                0.,    0.,     1.);
                int octaves = 3;
    
                float f = 0.;
                float freq = 1.;
                float a = .85;
                for(int i = 0; i < octaves; i++){
                    f += (1. - noise(p * freq)) * a;
                    a /= 2.;
                    freq *= 2.;
                    p = mul(q,p);
                    p += _Time[1]/5.;
                }
                return clamp((f * 2.) - 1.,0.,1.);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos= ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                o.viewDir = mul (unity_CameraInvProjection, float4 (o.uv * 2. - 1., 1., 1.));
                float3 worldDir = mul(unity_CameraInvProjection, float4(v.uv * 2. - 1., 0., -1.));
                o.worldDir = normalize(mul(unity_CameraToWorld, float4(worldDir,0)));
                return o;
            }


            //returns true if point is inside grid
            bool insideGrid (float3 p){
              p -= _Origin;
              if(p.x < 0. || p.y < 0. || p.z < 0.) return false;
              if(p.x > _Dim.x|| p.y > _Dim.y || p.z > _Dim.z) return false;
              return true;
            }

            //check voxel densities and use trilinear aliasing
            float sampleGrid (float3 p){
              if(!insideGrid(p)) return 0.;
              float2 z = float2(1,0);
              float3 F = frac(p);
              float3 index =  (p - _Origin)/_Dim;
              float a = tex3Dlod(_Grid, float4( index + z.yyy, 0)).w;
              float b = tex3Dlod(_Grid, float4( index + z.xyy, 0)).w;
              float c = tex3Dlod(_Grid, float4( index + z.yxy , 0)).w;
              float d = tex3Dlod(_Grid, float4( index + z.xxy , 0)).w;
              float e = tex3Dlod(_Grid, float4( index + z.yyx , 0)).w;
              float f = tex3Dlod(_Grid, float4( index + z.xyx , 0)).w;
              float g = tex3Dlod(_Grid, float4( index + z.yxx , 0)).w;;
              float h = tex3Dlod(_Grid, float4( index + z.xxx , 0)).w;


              F = F*F*(3.-2.*F);
              return lerp(lerp(lerp(a,b,F.x),
                               lerp(c,d,F.x),F.y),
                          lerp(lerp(e,f,F.x),
                               lerp(g,h,F.x),F.y),F.z);
            }

            //check if voxels are in light and uses trilinear aliasing
            float sampleLight (float3 p){
              if(!insideGrid(p)) return 0.;
              float2 z = float2(1,0);
              float3 F = frac(p);
              float3 index =  (p - _Origin)/_Dim;
              float a = tex3Dlod(_SunBuffer, float4( index + z.yyy, 0)).w;
              float b = tex3Dlod(_SunBuffer, float4( index + z.xyy, 0)).w;
              float c = tex3Dlod(_SunBuffer, float4( index + z.yxy , 0)).w;
              float d = tex3Dlod(_SunBuffer, float4( index + z.xxy , 0)).w;
              float e = tex3Dlod(_SunBuffer, float4( index + z.yyx , 0)).w;
              float f = tex3Dlod(_SunBuffer, float4( index + z.xyx , 0)).w;
              float g = tex3Dlod(_SunBuffer, float4( index + z.yxx , 0)).w;;
              float h = tex3Dlod(_SunBuffer, float4( index + z.xxx , 0)).w;

              
              F = F*F *(3. - 2.*F);
              return lerp(lerp(lerp(a,b,F.x),
                               lerp(c,d,F.x),F.y),
                          lerp(lerp(e,f,F.x),
                               lerp(g,h,F.x),F.y),F.z);
            }

            //cast ray to bounding volume
            float gridCast(inout float3 ro, float3 rd){
                if(insideGrid(ro)) return 0.;

                float3 originDist =  _Origin - ro;
                float3 dimDist = (_Dim - (ro - _Origin));

                float xDist = (rd.x>0.)? originDist.x/rd.x : dimDist.x/rd.x;
                float yDist = (rd.y>0.)? originDist.y/rd.y : dimDist.y/rd.y;
                float zDist = (rd.z>0.)? originDist.z/rd.z : dimDist.z/rd.z;
                xDist = xDist<0 ? 100000:xDist;
                yDist = yDist<0 ? 100000:yDist;
                zDist = zDist<0 ? 100000:zDist;

               
                float3 p = ro + rd * (xDist+.01);
                if(!insideGrid(p)) xDist = 10000;
                p = ro + rd * (yDist+.01);
                if(!insideGrid(p)) yDist = 10000;
                p =ro + rd * (zDist+.01);
                if(!insideGrid(p)) zDist = 10000;

                float minDist = min(min(xDist,yDist),zDist);
                ro += rd * (minDist + .01);
                if(insideGrid(ro)) return minDist;

                return -1;
                
            }

            //march towards light
            float3 lightMarch(float3 ro, float3 lightPos, bool isDirectional){
                float total = 0.;

                float dstep = 1.;
                float t = 0.;
                float3 rd = -normalize(lightPos - ro);
                float maxT = length(lightPos - ro);
                float transmittance = 1.;

                for(int i = 0; i < 5; i++){
                    t += dstep;
                    float3 p = ro + rd * t;
                    float density = pFbm(p) * sampleGrid(p);
                    if(density > .01) {
                        if(isDirectional){
                            transmittance *= exp(-density * dstep);
                        }else{
                            float dist = distance(p, lightPos)/8.;
                            transmittance *= exp(-density * dstep)*(dist)*(dist);
                        }
                    }

                    if(!insideGrid(p) || t > maxT) break;
                }
                
                if(!isDirectional){
                    return clamp(1. - transmittance,0.,1.);
                }
                return transmittance;
            }

            //march through volumetric
            float4 smokeMarch(float3 ro, float3 rd, float3 camPos, float depth){
                float3 accum = 0.;
                float transmittance = 1.;
                float dstep = 1.;
                float t = 0.;

                float ambient = .3;
                float brightness = .8;
                ro += rd * rand31(ro);
                for(int i = 0; i < 30; i++){
                    t += dstep;
                    float3 p = ro + rd * t;
                    float curDepth = distance(p,camPos);
                    float density = 0;

                    if(curDepth < depth) density = pFbm(p) * sampleGrid(p);
                    if(density > .01){
                        
                        //transmittance at current step
                        float dTrans = exp(-density* dstep);

                        //integral of light w.r.t transmittance
                        //credit https://www.ea.com/frostbite/news/physically-based-unified-volumetric-rendering-in-frostbite
                        float3 S = lightMarch(p,_SunDir * -10000., true) * density * brightness;
                        S *= sampleLight(p);

                        float3 Sint = (S - S * dTrans) * (1. / density) * .8;
                        accum += transmittance * Sint; 

                        //light march to flash
                        if(_Intensity.x > 0.){
                            float3 L = lightMarch(p,_LightPos, false) * density * brightness;
                            float3 Lint = (L - L * dTrans) * (1. / density) * _Intensity;
                            accum += transmittance * Lint; 
                        }
                        if(_Intensity2.x > 0.){
                            float3 L = lightMarch(p,_LightPos2, false) * density * brightness;
                            float3 Lint = (L - L * dTrans) * (1. / density) * _Intensity2;
                            accum += transmittance * Lint; 
                        }

                        transmittance *= dTrans;
                    }
                    if(!insideGrid(p)) break;
                }
                accum += ambient;
                return float4(accum, transmittance);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv);
                //credit https://forum.unity.com/threads/converting-depth-values-to-distances-from-z-buffer.921929/
                float depth = Linear01Depth (tex2D (_CameraDepthTexture, i.uv).r);
                float3 viewPos = (i.viewDir.xyz / i.viewDir.w) * depth;
                depth = length (viewPos);


                float3 rd = i.worldDir;
                float3 camOrigin = _WorldSpaceCameraPos;
                float3 ro = _WorldSpaceCameraPos;
                
                float dist = gridCast(ro,rd);
                if(dist.x < 0.){
                    return original;
                }
                float4 col = smokeMarch(ro, rd, camOrigin, depth);

                return float4(lerp(col.xyz, original.xyz, col.w),1.);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
