using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
	
	public class MatchMemberList : NetworkBehaviour
	{
		public static MatchMemberList Instance;

        private void Awake()
        {
			Instance = this;
        }

        [SerializeField] public List<MatchMemberData> allMemberData = new List<MatchMemberData>();

		public int MemberDataCount => allMemberData.Count;

		public static UnityAction<List<MatchMemberData>> UpdateList;

        public override void OnStartClient()
        {
            base.OnStartClient();
			allMemberData.Clear();
        }

        [Server]
		public void SvAddPlayer(MatchMemberData data)
		{
			allMemberData.Add(data);

			RpcClearPlayerDataList();

			for (int i = 0;  i < allMemberData.Count; i++)
            {
				RpcAddPlayer(allMemberData[i]);
            }
		}

		[Server]
		public void SvRemovePlayer(MatchMemberData data)
		{
			for (int i = 0; i < allMemberData.Count; i++)
			{
				if(allMemberData[i].Id == data.Id)
                {
					allMemberData.RemoveAt(i);
					break;
                }
			}

			RpcRemovePlayer(data);
		}

		[ClientRpc]
		private void RpcClearPlayerDataList()
		{
			if (isServer == true) return;

			allMemberData.Clear();
		}

		[ClientRpc]
		private void RpcAddPlayer(MatchMemberData data)
		{
			if(isClient == true && isServer == true)
            {
				UpdateList?.Invoke(allMemberData);
				return;
			}

			allMemberData.Add(data);

			UpdateList?.Invoke(allMemberData);
		}

		[ClientRpc]
		private void RpcRemovePlayer(MatchMemberData data)
		{
			for (int i =0; i < allMemberData.Count; i++)
            {
				if(allMemberData[i].Id == data.Id)
                {
					allMemberData.RemoveAt(i);
					break;
                }
            }

			UpdateList?.Invoke(allMemberData);
		}
	}



