using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace Wraith
{
    public class Module : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<Behaviour>();
        }
    }

    public class Behaviour : MonoBehaviour
    {
        Transform flipping;
        private Item item;
        private RagdollHand handLeft;
        private RagdollHand handRight;
        private RagdollHand lastHand;
        private float offset;
        public void Start()
        {
            item = GetComponent<Item>();
            item.OnDespawnEvent += Item_OnDespawnEvent;
            item.OnHandleReleaseEvent += Item_OnHandleReleaseEvent;
            handLeft = Player.local.handLeft.ragdollHand;
            handRight = Player.local.handRight.ragdollHand;
            offset = 1;
            flipping = item.transform.GetChild(1);
        }

        private void Item_OnHandleReleaseEvent(Handle handle, RagdollHand ragdollHand, bool throwing)
        {
            lastHand = ragdollHand;
            handLeft.playerHand.controlHand.OnButtonPressEvent += ControlHand_OnButtonPressEventLeft;
            handRight.playerHand.controlHand.OnButtonPressEvent += ControlHand_OnButtonPressEventRight;
        }

        private void Item_OnDespawnEvent(EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                handLeft.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventLeft;
                handRight.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventRight;
            }
        }


        private void ControlHand_OnButtonPressEventRight(PlayerControl.Hand.Button button, bool pressed)
        {
            if (button == PlayerControl.Hand.Button.AlternateUse && lastHand == handRight)
            {
                Vector3 newposition = item.transform.position - (flipping.transform.up.normalized * offset);
                Player.local.transform.position = newposition;
                handRight.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventRight;
            }
            else
            {
                item.OnGrabEvent += Item_OnGrabEvent;
            }
        }
        private void ControlHand_OnButtonPressEventLeft(PlayerControl.Hand.Button button, bool pressed)
        {
            if (button == PlayerControl.Hand.Button.AlternateUse && lastHand == handLeft)
            {
                Vector3 newposition = item.transform.position - (flipping.transform.up.normalized * offset);
                Player.local.transform.position = newposition;
                handLeft.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventLeft;
            }
            else
            {
                item.OnGrabEvent += Item_OnGrabEvent;
            }
        }

        private void Item_OnGrabEvent(Handle handle, RagdollHand ragdollHand)
        {
            handRight.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventRight;
            handLeft.playerHand.controlHand.OnButtonPressEvent -= ControlHand_OnButtonPressEventLeft;
        }
    }
}