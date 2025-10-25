using System;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    private PlayerStats _playerStats;
    [SerializeField] private Slider _xpBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    void Start()
    {
        setValues();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void increaseXP(float xp)
    {
        print("increase");
        _xpBar.value += xp;
    }

    public void levelUp(float newValue)
    {
        _xpBar.maxValue = newValue;
        _xpBar.value = 0;
    }

    public void setValues()
    {
        _xpBar.maxValue = _playerStats.xpToNextLevelValue;
        _xpBar.value = _playerStats.CurrentXP;
        print("setou");
    }
}
