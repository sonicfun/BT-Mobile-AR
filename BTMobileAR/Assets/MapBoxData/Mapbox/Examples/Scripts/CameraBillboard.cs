namespace Mapbox.Examples
{
	using UnityEngine;

	public class CameraBillboard : MonoBehaviour
	{
		public Camera _camera;
        

        public void Start()
		{
			_camera = Camera.main;
            

            foreach (var cc in Camera.allCameras)
            {
                if (cc.tag == "MapCamera")
                    _camera = cc;
            }
        }

		void Update()
		{
			transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
		}
	}
}