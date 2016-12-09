;ここはコメントになります

[setskin ref="skin1"]

[background ref = "1000"]
[bgm ref = "000_title"]
*main
[loadcg id=0 ref = "alice"]
[loadcg id=1 ref = "alice"]
[loadcg id=2 ref = "alice"]
[loadfg id=0 ref="alice"]
[p]
[drawcg id=0 pos="right" dir="right" mode="slidein" layer="back"]
[drawcg id=1 posx="100" posy="100" state="1"]
[drawcg id=2 pos="left" mode="fadein" state="2"]
[p]
#アリス
[drawfg id=0 state="2"]
顔グラのテストだ[p]
[changefg state="1"]
#アリス
表情の変更もできるぞ。[p]
[clearfg]
[cn]
終了します[p]
