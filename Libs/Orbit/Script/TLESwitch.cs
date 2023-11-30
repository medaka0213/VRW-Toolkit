using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using UdonToolkit;

public class TLESwitch : UdonSharpBehaviour
{
    [UdonSynced]public int currentIndex;
    private int _currentIndex = -1;

    [ListView("TLE Text List")] 
    public Text[] TLEs;
    public Orbit target;
    
    public void Update(){
        if (currentIndex != _currentIndex) {
            _currentIndex = currentIndex;
            SetTLE(currentIndex);
        }
    }

    public void SetTLE(int index){
        //currentIndexが0以上かつTLEsの要素数以上ならば修正
        if (index < 0) index = 0;
        if (index >= TLEs.Length) index = TLEs.Length - 1;

        TLEs[index].gameObject.SetActive(true);
        target.TLEReferance = TLEs[index];
        target.ParseTLE();
    }

    public void SetIndex(int index){
        TakeOwnership();
        currentIndex = index;
    }

    public void Set0(){
        SetIndex(0);
    }
    public void Set1(){
        SetIndex(1);
    }
    public void Set2(){
        SetIndex(2);
    }
    public void Set3(){
        SetIndex(3);
    }
    public void Set4(){
        SetIndex(4);
    }
    public void Set5(){
        SetIndex(5);
    }
    public void Set6(){
        SetIndex(6);
    }
    public void Set7(){
        SetIndex(7);
    }
    public void Set8(){
        SetIndex(8);
    }
    public void Set9(){
        SetIndex(9);
    }

    public void Set10(){
        SetIndex(10);
    }

    public void Set11(){
        SetIndex(11);
    }

    public void Set12(){
        SetIndex(12);
    }

    public void Set13(){
        SetIndex(13);
    }

    public void Set14(){
        SetIndex(14);
    }

    public void Set15(){
        SetIndex(15);
    }

    public void Set16(){
        SetIndex(16);
    }

    public void Set17(){
        SetIndex(17);
    }

    public void Set18(){
        SetIndex(18);
    }

    public void Set19(){
        SetIndex(19);
    }

    public void Set20(){
        SetIndex(20);
    }

    public void TakeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;
}
