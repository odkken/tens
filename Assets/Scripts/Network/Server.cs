using UnityEngine;

namespace Assets.Scripts
{
    public class Server : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            Network.InitializeServer(10, 28800, true);
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
