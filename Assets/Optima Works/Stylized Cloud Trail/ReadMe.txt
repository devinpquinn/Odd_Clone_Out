Thank you for purchasing this asset!

If you have any problems\questions regarding this asset please contact us by e-mail: optima.works.assets@gmail.com

Troubleshooting:

Particles Not Working: 
-Trail particle uses distance-based emission. They emit when moved. If you want to change this, go to the "Emission" section and carry the value from "Distance" to "Time"
Landing (Explosion) particle needs to be called manually. Here is an example of how to call it;
========================================
public ParticleSystem landingParticle;

void OnLanding(){
landingParticle.Play();
}

Make sure to assign the landing particle from inspector. Assuming that you already have a functioning landing function.
========================================

Particle looks flat/weird: 
-This particle emits 3D meshes, which means it is effected by the lightning settings on your scene. You might want to adjust the materials or lightning settings until it is a fit for you.

Particle looks all pink: 
-This might be due to material shader mismatch. By default, the materials use URP shaders. If you are using a different render pipeline, you need to assign new materials in that pipeline.

Particle causes FPS to drop:
-If you are having any FPS drops/performance issues, you might want to adjust the "Max Particles" setting or Emission rates. Mesh we have used in this particle has less than 500 triangles, which means you are not likely
to run into any issues unless you have increased emission drastically.

If your issue wasn't listed here, please feel free to reach us at optima.works.assets@gmail.com