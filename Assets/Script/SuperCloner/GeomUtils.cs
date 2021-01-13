using UnityEngine;

namespace Utils.SuperCloner {
    
    public static class GeomUtils {

        public static Vector3 ExtractRight(Matrix4x4 matrix) => 
            new Vector3(matrix.m00, matrix.m10, matrix.m20);            

        public static Vector3 ExtractUp(Matrix4x4 matrix) => 
            new Vector3(matrix.m01, matrix.m11, matrix.m21);            

        public static Vector3 ExtractForward(Matrix4x4 matrix) => 
            new Vector3(matrix.m02, matrix.m12, matrix.m22);            

        public static Quaternion ExtractRotation(Matrix4x4 matrix) {

            Vector3 forward = ExtractForward(matrix);
            Vector3 upwards = ExtractUp(matrix);
            return Quaternion.LookRotation(forward, upwards);
        }
    
        public static Vector3 ExtractPosition(Matrix4x4 matrix) {

            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }
    
        public static Vector3 ExtractScale(Matrix4x4 matrix) {

            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}