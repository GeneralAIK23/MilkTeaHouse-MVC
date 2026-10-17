(function () {
  "use strict";
  var root = document.getElementById("mth-topping-config");
  if (!root) return;
  var rows = Array.prototype.slice.call(root.querySelectorAll(".mth-topping-row"));
  var quantity = root.querySelector("#quantity");
  var money = function (n) { return Number(n).toLocaleString("vi-VN") + " ₫"; };
  var base = Number(root.getAttribute("data-base-price")) || 0;
  var large = Number(root.getAttribute("data-large-extra")) || 0;
  function clamp(n, max) {
    n = Number(n);
    if (!Number.isFinite(n) || !Number.isInteger(n)) return 0;
    return Math.max(0, Math.min(max, n));
  }
  function refresh() {
    var toppingTotal = 0, toppingCount = 0;
    rows.forEach(function (row) {
      var input = row.querySelector(".mth-topping-qty");
      var amount = clamp(input.value, 5);
      input.value = amount;
      toppingCount += amount;
      toppingTotal += amount * (Number(row.getAttribute("data-price")) || 0);
    });
    var sizeL = root.querySelector('input[name="size"][value="L"]');
    var unit = base + (sizeL && sizeL.checked ? large : 0) + toppingTotal;
    var cups = clamp(quantity.value, 30);
    if (cups < 1) cups = 1;
    document.getElementById("mth-topping-total").textContent = money(toppingTotal);
    document.getElementById("mth-unit-total").textContent = money(unit);
    document.getElementById("mth-grand-total").textContent = money(unit * cups);
    return toppingCount;
  }
  rows.forEach(function (row) {
    var input = row.querySelector(".mth-topping-qty");
    row.querySelector(".mth-minus").addEventListener("click", function () {
      input.value = Math.max(0, clamp(input.value,5) - 1); refresh();
    });
    row.querySelector(".mth-plus").addEventListener("click", function () {
      var current = refresh();
      if (current >= 20) return;
      input.value = Math.min(5, clamp(input.value,5) + 1); refresh();
    });
    input.addEventListener("change", function () {
      input.value = clamp(input.value,5);
      if (refresh()>20) { input.value=0; refresh(); window.alert("Tối đa 20 phần topping cho mỗi ly."); }
    });
  });
  root.querySelectorAll('input[name="size"]').forEach(function (r) { r.addEventListener("change", refresh); });
  if (quantity) quantity.addEventListener("input", refresh);
  var clear=root.querySelector(".mth-toppings-clear");
  if(clear) clear.addEventListener("click", function () {rows.forEach(function(row){row.querySelector(".mth-topping-qty").value=0;});refresh();});
  var form=root.closest("form");
  if(form) form.addEventListener("submit",function(e) {
    var qty=Number(quantity.value);
    if(!Number.isInteger(qty) || qty<1 || qty>30 || refresh()>20) {
      e.preventDefault(); window.alert("Vui lòng kiểm tra số lượng ly và topping.");
    }
  });
  refresh();
})();
