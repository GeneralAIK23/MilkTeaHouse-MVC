(function(){
 'use strict';
 var button=document.getElementById('mth-promo-check');
 if(!button)return;
 var input=document.getElementById('mth-promo-code'),
 status=document.getElementById('mth-promo-status'),
 discountEl=document.getElementById('mth-promo-discount'),
 totalEl=document.getElementById('mth-promo-grand');
 var defaultTotal=totalEl.getAttribute('data-original');
 function money(num){return Number(num).toLocaleString('vi-VN')+' đ';}
 function reset(){discountEl.textContent='0 đ';totalEl.textContent=money(defaultTotal);}
 input.addEventListener('input',function(){status.textContent='Nhấn Kiểm tra mã để xem mức giảm.';reset();});
 button.addEventListener('click',function(){
  var token=document.querySelector('input[name="__RequestVerificationToken"]');
  if(!token){status.textContent='Thiếu mã chống gửi giả mạo.';return;}
  status.textContent='Đang kiểm tra...';button.disabled=true;
  var data='__RequestVerificationToken='+encodeURIComponent(token.value)+'&promoCode='+encodeURIComponent(input.value);
  fetch(button.getAttribute('data-url'),{method:'POST',credentials:'same-origin',headers:{'Content-Type':'application/x-www-form-urlencoded; charset=UTF-8'},body:data})
   .then(function(r){if(!r.ok)throw Error('Không thể kiểm tra mã. Vui lòng đăng nhập và thử lại.');return r.json();})
   .then(function(p){if(!p.valid){reset();status.textContent=p.message||'Mã không hợp lệ.';return;}discountEl.textContent='−'+money(p.discount);totalEl.textContent=money(p.total);status.textContent=p.message;})
   .catch(function(e){reset();status.textContent=e.message;})
   .finally(function(){button.disabled=false;});
 });
})();
