# ロケみる集会ツールキット

「ロケット打ち上げを観る集会」の開場作成のためのUnityアセット置き場

## 依存関係

Unity version 2022.3.6f1

VCCで導入可能なもの

| 名前 | バージョン |
| --- | --- |
| VRChat SDK - Base | 3.5.0 |
| VRChat SDK - Worlds | 3.5.0 |
| liltoon https://lilxyzw.github.io/lilToon/#/  | 1.7.3 |
| QVPen https://vpm.ureishi.net/install | 3.2.9 |

UnityPackageのインポートが必要なもの

| 名前 | バージョン |
| --- | --- |
| IwaSync3 https://booth.pm/ja/items/2666275 | 3.5.7(U#1.0) |
| UdonToolkit https://github.com/orels1/UdonToolkit/releases | 1.2.1 |
| 【VRC想定】レーザーポインター https://booth.pm/ja/items/1320191 | 1.02 |

## 使い方

### レイヤーの設定

軌道モデルのライティングのために、レイヤーを追加する。
参考: https://docs.unity3d.com/ja/2019.4/Manual/Layers.html

Edit > Project Settings を開き、Tags and Layers を選択して、レイヤーを追加<br>
- User Layer 23: `Earth`
- User Layer 24: `EarthRealScale`

![image](https://github.com/medaka0213/VRW-Toolkit/assets/36759068/6e459f7e-b5c6-4ade-8dd9-e71cbb256d09)

シーンのDirectional Lightの設定<br>
Culling Maskから`Earth` `EarthRealScale`を除外

![image](https://github.com/medaka0213/VRW-Toolkit/assets/36759068/1c5c9ed0-d824-4d0e-8501-86296b6fbf63)


### 開場の設営

対象のワールドに`Medaka/Prefabs/VRW System/VRW System.prefab`を配置する。
