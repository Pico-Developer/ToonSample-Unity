Toon World is a performance optimization demonstration. It uses multiview rendering, application spacewarp, and late latching to balance the app's performance consumption while maintaining a high-quality rendering effect.

![image](https://github.com/Pico-Developer/ToonSample-Unity/assets/110143438/d12de6af-36ad-435e-a90f-4a7a47228d21)

## Development environment
- Unity Editor: 2021.3.27f1
- PICO Unity Integration SDK: 2.1.4
- PICO Device: PICO 4
- PICO Device's System Version: 5.5.0
- Graphics API: Vulkan (OpenGL is not supported)
- Input System: 1.6.1
- Rendering Pipeline: Universal Render Pipeline, customized on version 12.1.6, including the following resources : Core RP Library、Universal RP、Shader Graph
- XR Interaction Toolkit: 2.3.2
- XR Plugin Management: 4.3.3

## Project settings
| Item | Setting | 
| --- | --- |
| Stereo Rendering Mode | Multiview |
| URP Render Scale | 1x |
| MSAA | 4x |
| Number of light sources | 1x Directional Light |
| Baked Lightmaps | On |
| Post processing | On: Color Lookup, Color Adjustments |
| HDR | Off |
| Graphics API | Vulkan |

## Performance stats 
|  | Before | After |
| --- | --- | --- |
| FPS | 72 | 36 |
| Grass Density Level| Low | High |

## Optimization methods
- Multiview rendering
- Application spacewarp
- Late latching

## Related articles
- For more information on the Toon World demo, refer to [this article](https://developer-global.pico-interactive.com/document/unity/toon-world/).
- [Multiview rendering guide](https://developer-global.pico-interactive.com/document/unity/multiview-rendering/)
- [Application spacewarp guide](https://developer-global.pico-interactive.com/document/unity/application-spacewarp/)
- [Late latching guide](https://developer-global.pico-interactive.com/document/unity/late-latching/)
