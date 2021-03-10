using Photon.Pun.Simple;
using UnityEngine.UI;

namespace Photon.Pun.Demo
{
    public class OwnerGUI : NetComponent
    {
        Text text;

        // Use this for initialization
        public override void OnAwake()
        {
            base.OnAwake();
            text = GetComponentInChildren<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (text)
            {
                var pv = photonView;
                text.text = pv.OwnerActorNr + " : " + pv.ControllerActorNr;
            }

        }
    }

}
