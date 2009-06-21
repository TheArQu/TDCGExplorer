using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TAHdecrypt
{

public class TSOCamera
{
    internal Vector3 center = Vector3.Empty;    //回転中心位置
    internal Vector3 translation = Vector3.Empty;       //view座標上の位置
    internal Vector3 camPosL = new Vector3(0.0f, 0.0f, -10.0f); //カメラ位置
    internal Vector3 camDirDef = Vector3.Empty; //カメラ移動方向ベクトル
    internal float offsetZ = 0.0f;      //カメラ奥行オフセット値
    internal bool needUpdate = true;    //更新したか
    internal Matrix viewMat = Matrix.Identity;  //ビュー行列
    internal Matrix camPoseMat = Matrix.Identity;       //カメラ姿勢行列
    internal float camZRotDef = 0.0f;   //カメラ Z軸回転差分
    internal float camAngleUnit = 0.02f;        //移動時回転単位（ラジアン）

    public Vector3 Center { get { return center; } set { center = value; } }
    public Vector3 Translation { get { return translation; } set { translation = value; } }
    public Vector3 CamPosL { get { return camPosL; } set { camPosL = value; } }
    public Matrix CamPoseMat { get { return camPoseMat; } set { camPoseMat = value; } }

    public TSOCamera()
    {
        motion = new TSOCameraMotion(this);
    }

    /// <summary>カメラ位置と姿勢を標準出力へ書き出す</summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TSOCamera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>カメラ位置と姿勢を書き出す</summary>
    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TSOCamera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>カメラ位置と姿勢を読み込む</summary>
    public static TSOCamera Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(TSOCamera));
        TSOCamera camera = serializer.Deserialize(reader) as TSOCamera;
        reader.Close();
        return camera;
    }

    /// <summary>カメラ位置と姿勢を補間する</summary>
    public static TSOCamera Interpolation(TSOCamera cam1, TSOCamera cam2, float ratio)
    {
        TSOCamera camera = new TSOCamera();
        camera.Center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        camera.Translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        camera.CamPosL = Vector3.Lerp(cam1.CamPosL, cam2.CamPosL, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.CamPoseMat);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.CamPoseMat);
        camera.CamPoseMat = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));
        return camera;
    }

    public void Interp(TSOCamera cam1, TSOCamera cam2, float ratio)
    {
        center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        camPosL = Vector3.Lerp(cam1.CamPosL, cam2.CamPosL, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.CamPoseMat);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.CamPoseMat);
        camPoseMat = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));
        needUpdate = true;
    }

    /// <summary>カメラ位置と姿勢をリセット</summary>
    public void Reset()
    {
        center = Vector3.Empty;
        translation = Vector3.Empty;
        camPosL = new Vector3(0.0f, 0.0f, -10.0f);
        camPoseMat = Matrix.Identity;
        needUpdate = true;
    }

    /// <summary>カメラ位置更新</summary>
    /// <param name="camDirX">移動方向（経度）</param>
    /// <param name="camDirY">移動方向（緯度）</param>
    /// <param name="offsetZ">奥行オフセット値</param>
    public void Move(float camDirX, float camDirY, float offsetZ)
    {
        if (camDirX == 0.0f && camDirY == 0.0f && offsetZ == 0.0f)
            return;

        camDirDef.X += camDirX;
        camDirDef.Y += camDirY;
        this.offsetZ += offsetZ;
        needUpdate = true;
    }

    /// <summary>Z軸回転</summary>
    public void RotZ(float radian)
    {
        if (radian == 0.0f)
            return;

        camZRotDef = radian;
        needUpdate = true;
    }

    public void LookAt(Vector3 eye, Vector3 center, Vector3 up)
    {
        this.camPosL = center - eye;
        {
            // カメラ姿勢を更新
            Vector3 z = Vector3.Normalize(-camPosL);
            Vector3 y = up;
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                this.camPoseMat = m;
            }
        }
        this.center = Vector3.Empty;
        this.translation = eye;

        //view行列更新
        Vector3 posW = camPosL + center;
        {
            Matrix m = camPoseMat;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            viewMat = Matrix.Invert(m) * Matrix.Translation(-translation);
        }
    }
    public void LookAt(Vector3 eye, Vector3 center)
    {
        LookAt(eye, center, new Vector3(0.0f, 1.0f, 0.0f));
    }

    /// <summary>カメラ更新</summary>
    public void Update()
    {
        if (! needUpdate)
            return;

        //カメラ Z軸回転で姿勢を仮更新
        camPoseMat = Matrix.RotationZ(camZRotDef) * camPoseMat;

        //緯度経度の差分移動
        Vector3 dL = Vector3.TransformCoordinate(camDirDef, camPoseMat);
        if (dL.X != 0.0f || dL.Y != 0.0f || dL.Z != 0.0f)
        {
            Vector3 camZAxis = new Vector3(camPoseMat.M31, camPoseMat.M32, camPoseMat.M33);
            Vector3 rotAxis = Vector3.Cross(dL, camZAxis);
            Quaternion q = Quaternion.RotationAxis(rotAxis, camAngleUnit * camDirDef.Length());
            Matrix rotMat = Matrix.RotationQuaternion(q);
            camPosL = Vector3.TransformCoordinate(camPosL, rotMat);

            // 移動後カメラ姿勢を更新
            Vector3 z = Vector3.Normalize(-camPosL);
            Vector3 y = new Vector3(camPoseMat.M21, camPoseMat.M22, camPoseMat.M23);
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                camPoseMat = m;
            }
        }

        // 奥行オフセットを更新
        if (camPosL.Length() - offsetZ > 0)
        {
            Vector3 z = Vector3.Normalize(-camPosL);
            camPosL += offsetZ * z;
        }

        //view行列更新
        Vector3 posW = camPosL + center;
        {
            Matrix m = camPoseMat;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            viewMat = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //差分をリセット
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>view行列を取得</summary>
    public Matrix GetViewMatrix()
    {
        return viewMat;
    }

    /// <summary>回転中心位置を設定</summary>
    public void SetCenter(Vector3 center)
    {
        this.center = center;
        needUpdate = true;
    }

    /// <summary>view座標上の位置を設定</summary>
    public void SetTranslation(Vector3 translation)
    {
        this.translation = translation;
        needUpdate = true;
    }

    /// <summary>view座標上で移動</summary>
    public void MoveView(float dx, float dy)
    {
        this.translation.X += dx;
        this.translation.Y += dy;
        needUpdate = true;
    }

    // 差分をリセット
    protected void ResetDefValue()
    {
        camDirDef = Vector3.Empty;
        offsetZ = 0.0f;
        camZRotDef = 0.0f;
    }

    private TSOCameraMotion motion = null;

    public TSOCameraMotion Motion
    {
        get { return motion; }
    }

    public void SetMotion(int frame_index, Vector3 eye, Vector3 center)
    {
        motion.Add(frame_index, eye, center);
    }
    public void SetMotion(int frame_index, Vector3 eye, Vector3 center, int interp_length)
    {
        motion.Add(frame_index, eye, center, interp_length);
    }
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz));
    }
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz, int interp_length)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz), interp_length);
    }

    public void NextFrame()
    {
        if (motion.Count != 0)
        {
            motion.NextFrame();
        }
    }
}
}
