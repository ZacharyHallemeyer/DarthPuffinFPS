using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    // Grapple UI
    public Slider grappleSlider;
    public Gradient grappleGradient;
    public Image grappleFill;
    public TextMeshProUGUI grappleOutOfBoundsText;

    // Ammo UI
    public TextMeshProUGUI ammoText;

    // JetPack UI
    public Slider jetPackSlider;
    public Gradient jetPackGradient;
    public Image jetPackFill;

    // Grapple =============================
    public void SetMaxGrapple(float maxValue)
    {
        grappleSlider.maxValue = maxValue;
        grappleSlider.value = maxValue;

        grappleFill.color = grappleGradient.Evaluate(1f);
    }

    public void SetGrapple(float grappleTime)
    {
        grappleSlider.value = grappleTime;
        grappleFill.color = grappleGradient.Evaluate(grappleSlider.normalizedValue);
    }

    public void GrappleOutOfBoundsUI()
    {
        grappleOutOfBoundsText.text = "X";
        InvokeRepeating("HideGrappleOutOfBoundsUI", 1f, 0f);
    }

    private void HideGrappleOutOfBoundsUI()
    {
        grappleOutOfBoundsText.text = "";
        CancelInvoke("HideGrappleOutOfBoundsUI");
    }

    // End of Grapple ======================

    // Ammo ================================

    public void ChangeGunUIText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    // End of Ammo =========================

    // Jet Pack ============================
    public void SetMaxJetPack(float maxValue)
    {
        jetPackSlider.maxValue = maxValue;
        jetPackSlider.value = maxValue;

        jetPackFill.color = jetPackGradient.Evaluate(1f);
    }

    public void SetJetPack(float jetPackTime)
    {
        jetPackSlider.value = jetPackTime;
        jetPackFill.color = jetPackGradient.Evaluate(jetPackSlider.normalizedValue);
    }

    // End of Jet Pack =====================
}
