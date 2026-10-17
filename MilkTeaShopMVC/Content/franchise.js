(function(){
  function ready(){
    var modal=document.getElementById('mtsFranchiseModal');
    if(!modal) return;
    var lastFocus=null;
    function close(){ modal.hidden=true;document.body.style.overflow=''; if(lastFocus&&lastFocus.focus)lastFocus.focus(); }
    function open(trigger){lastFocus=trigger;modal.hidden=false;document.body.style.overflow='hidden';var first=modal.querySelector('input[name="FullName"]');if(first)first.focus();}
    document.addEventListener('click',function(e){
      var opener=e.target.closest&&e.target.closest('[data-mts-franchise-open]');
      if(opener){e.preventDefault();open(opener);return;}
      if(e.target===modal || (e.target.closest&&e.target.closest('[data-mts-franchise-close]'))) close();
    });
    document.addEventListener('keydown',function(e){
      if(modal.hidden)return;
      if(e.key==='Escape'){e.preventDefault();close();return;}
      if(e.key==='Tab'){
        var nodes=modal.querySelectorAll('button:not([disabled]),a[href],input:not([disabled]):not([tabindex="-1"]),select:not([disabled]),textarea:not([disabled])');
        if(!nodes.length)return;
        var first=nodes[0],last=nodes[nodes.length-1];
        if(e.shiftKey&&document.activeElement===first){e.preventDefault();last.focus();}
        else if(!e.shiftKey&&document.activeElement===last){e.preventDefault();first.focus();}
      }
    });
  }
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',ready);else ready();
})();
