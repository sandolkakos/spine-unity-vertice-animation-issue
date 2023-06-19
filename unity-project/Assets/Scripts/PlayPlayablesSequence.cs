using System;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Sample
{
    public class PlayPlayablesSequence : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector playableDirector;

        [SerializeField]
        private SkeletonGraphic skeletonGraphic;

        [SerializeField]
        private AssetReferenceT<PlayableAsset>[] playableReferences;

        [SerializeField]
        private PlayableAsset[] playables;

        [SerializeField]
        private Button addressablesButton;

        [SerializeField]
        private Button nonAddressablesButton;

        private void Start()
        {
            addressablesButton.onClick.AddListener(PlayAddressablesSequence);
            nonAddressablesButton.onClick.AddListener(PlayNonAddressablesSequence);
        }

        private async void PlayAddressablesSequence()
        {
            try
            {
                foreach (var playableReference in playableReferences)
                {
                    await Play(playableReference);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async void PlayNonAddressablesSequence()
        {
            try
            {
                foreach (var playable in playables)
                {
                    await Play(playable);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task Play(AssetReferenceT<PlayableAsset> assetReference)
        {
            var operationHandle = assetReference.LoadAssetAsync<PlayableAsset>();
            PlayableAsset playableAsset = await operationHandle.Task;

            await Play(playableAsset);
            assetReference.ReleaseAsset();
        }

        private async Task Play(PlayableAsset playableAsset)
        {
            BindComponents(playableAsset);
            playableDirector.Play(playableAsset, DirectorWrapMode.Hold);
            await Task.Delay(TimeSpan.FromSeconds(playableAsset.duration));
        }

        private void BindComponents(IPlayableAsset playableAsset)
        {
            foreach (var playableAssetOutput in playableAsset.outputs)
            {
                if (playableAssetOutput.streamName != "SkeletonGraphic_Track")
                {
                    continue;
                }

                var sourceObject = playableAssetOutput.sourceObject;
                var bindingObject = skeletonGraphic;

                playableDirector.SetGenericBinding(sourceObject, bindingObject);
            }
        }
    }
}