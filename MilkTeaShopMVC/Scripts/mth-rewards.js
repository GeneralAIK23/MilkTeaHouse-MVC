(function(){
 'use strict';
 var points=document.getElementById('mth-points-input');
 var promo=document.getElementById('mth-promo-discount');
 var pointsDiscount=document.getElementById('mth-points-discount');
 var grand=document.getElementById('mth-promo-grand');
 var info=document.getElementById('mth-points-status');
 if(!points||!promo||!pointsDiscount||!grand) return;
 var original=Number(grand.getAttribute('data-original'))||0;
 var balance=Number(points.getAttribute('max'))||0;
 function format(n){return Number(n).toLocaleString('vi-VN')+' đ';}
 function discountText(){return Number((promo.textContent||'').replace(/[^0-9]/g,''))||0;}
 function update(){
  var entered=Number(points.value||0);
  var coupon=discountText();
  var remaining=Math.max(0,original-coupon);
  var max=Math.min(balance,Math.floor(remaining/200));
  var valid=Number.isInteger(entered) && entered>=0 && entered<=max;
  var use=valid?entered:0;
  var off=use*200;
  pointsDiscount.textContent=off>0?'−'+format(off):'0 đ';
  grand.textContent=format(remaining-off);
  info.textContent=valid?'Có thể đổi tối đa '+max+' điểm cho giỏ hàng hiện tại.':'Điểm không hợp lệ. Tối đa '+max+' điểm sau khuyến mãi.';
  info.style.color=valid?'#425775':'#b23b36';
 }
 points.addEventListener('input',update);
 // Script promo cu cap nhat DOM sau khi kiem tra ma.
 var observer=new MutationObserver(update);
 observer.observe(promo,{childList:true,characterData:true,subtree:true});
 update();
})();
