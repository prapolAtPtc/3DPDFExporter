using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfSharp3dAnnot
{
    public sealed class FitVisible
    {
        public static string FitVisibleJs
        {
            get
            {
                return (@"var camera = scene.cameras.getByIndex(0);
                var camTransform = camera.transform;
                var bbox = scene.computeBoundingBox();
                var boundingBox = 0;var isSelected = false;
                var VwVector = camera.position.subtract(camera.targetPosition);VwVector.normalize();
                var Bboxvector = bbox.max.subtract(bbox.min);
                if (camera.projectionType == ""perspective"")
                {
                    console.println(""perspective"");
                    var roll = camera.roll;
                    var dist = (Bboxvector.length / 2) / (Math.tan(camera.fov / 2));
                    dist *= 1.5175;
                    var vector = VwVector.scale(dist);
                    var newCamPosition = bbox.center.add(vector);
                    camera.position.set(newCamPosition);
                    camera.targetPosition.set(bbox.center);
                    camera.roll = roll;

                }
                else   // ortho
                {
                    var i = 0;
                    while (i < 2) {
                        console.println(""ortho"");
                        var roll = camera.roll;
                        var vector = VwVector.scale(camera.viewPlaneSize * 4);
                        var newCamPosition = bbox.center.add(vector);
                        camera.position.set(newCamPosition);
                        camera.targetPosition.set(bbox.center);
                        camera.viewPlaneSize = Bboxvector.length * 1.35;
                        camera.roll = roll;
                        i = i + 1;
                    }
                }"
             );
            }
        }
    }
}