using UnityEngine;
using System.Collections.Generic;

namespace ProjectWitch
{
    //サウンドの種類
    public enum SoundType { BGM, SE, Voice }

    //よく使うSE
    public enum SE { Click, Cancel, Hover }

    //サウンド管理クラス
    public class SoundManager : MonoBehaviour
    {

        //BGMのソースオブジェクト
        [SerializeField]
        private GameObject mBGMSource = null;
        private CriAtomSource mcBGM = null;

        //SEのソースオブジェクト
        [SerializeField]
        private GameObject mSESource = null;
        private CriAtomSource mcSE = null;

        //ボイスのソースオブジェクト
        [SerializeField]
        private GameObject mVoiceSource = null;
        private CriAtomSource mcVoice = null;

        //サウンドの種類とキューシート名の対応表
        private readonly Dictionary<SoundType, string> mCueSheetList
            = new Dictionary<SoundType, string>()
            {
            { SoundType.BGM,    "BGM" },
            { SoundType.SE,     "SE" },
            { SoundType.Voice,  "Voice" }
            };

        //よく使うSEの対応表
        private readonly Dictionary<SE, string> mSENameList
            = new Dictionary<SE, string>()
            {
            {SE.Click, "040_click" },
            {SE.Cancel, "041_cancel" },
            {SE.Hover, "042_hover" }
            };

        public void Start()
        {
            //エラーチェック
            Debug.Assert(mBGMSource, "BGMソースをセットしてください");
            Debug.Assert(mSESource, "SEソースをセットしてください");
            Debug.Assert(mVoiceSource, "Voiceソースをセットしてください");

            //コンポーネントの取得
            mcBGM = mBGMSource.GetComponent<CriAtomSource>();
            mcSE = mSESource.GetComponent<CriAtomSource>();
            mcVoice = mVoiceSource.GetComponent<CriAtomSource>();

            //エラーチェック
            Debug.Assert(mcBGM, "BGMソースにCriAtomSourceコンポーネントをセットしてください");
            Debug.Assert(mcSE, "SEソースにCriAtomSourceコンポーネントをセットしてください");
            Debug.Assert(mcVoice, "VoiceソースにCriAtomSourceコンポーネントをセットしてください");
        }

        public void Update()
        {
            var game = Game.GetInstance();

            //ボリュームの同期
            var master = game.SystemData.Config.MasterVolume;
            mcBGM.volume = game.SystemData.Config.BGMVolume * master;
            mcSE.volume = game.SystemData.Config.SEVolume * master;
            mcVoice.volume = game.SystemData.Config.VoiceVolume * master;
        }

        //再生開始
        public void Play(string name, SoundType type)
        {
            if (!IsExist(name, type))
            {
                Debug.LogWarning("指定したサウンドファイルは存在しません　" +
                    "　種別：" + mCueSheetList[type] +
                    "　名前：" + name);
                return;
            }

            var source = GetSource(type);

            //対象がBGMかつ、もし同じものを再生中なら、再スタートしない
            if (type==SoundType.BGM && IsPlaying(SoundType.BGM) &&
                source.cueName == name)
                return;

            source.cueSheet = mCueSheetList[type];
            source.cueName = name;

            source.Play();

        }

        //ポーズ(BGM限定) true:ポーズをかける false:ポーズ解除
        public void Pause(bool pause)
        {
            var source = GetSource(SoundType.BGM);
            source.Pause(pause);
        }

        //再生停止
        public void Stop(SoundType type)
        {
            var source = GetSource(type);

            source.Stop();
        }

        //指定のタイプのソースが再生中かどうか
        public bool IsPlaying(SoundType type)
        {
            var source = GetSource(type);

            return source.status == CriAtomSource.Status.Playing;
        }

        //キュー名を取得
        public string GetCueName(SoundType type)
        {
            var source = GetSource(type);

            return source.cueName;
        }

        //---------------
        //よく使うSEの専用再生
        //---------------
        public void PlaySE(SE SEType)
        {
            Play(mSENameList[SEType], SoundType.SE);
        }

        //指定のタイプのCriAtomSourceを取得
        private CriAtomSource GetSource(SoundType type)
        {

            CriAtomSource source = null;
            switch (type)
            {
                case SoundType.BGM: source = mcBGM; break;
                case SoundType.SE: source = mcSE; break;
                case SoundType.Voice: source = mcVoice; break;
                default: break;
            }

            Debug.Assert(source, "サウンドタイプが不正です");

            return source;
        }

        //指定のキュー名が存在するか
        private bool IsExist(string cueName, SoundType type)
        {
            CriAtomExAcb acb = null;
            switch (type)
            {
                case SoundType.BGM: acb = CriAtom.GetAcb("BGM"); break;
                case SoundType.SE: acb = CriAtom.GetAcb("SE"); break;
                case SoundType.Voice: acb = CriAtom.GetAcb("Voice"); break;
                default: break;
            }

            return acb.Exists(cueName);
        }

    }
}