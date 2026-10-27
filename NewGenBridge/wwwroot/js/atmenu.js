(() => {
 const table=document.querySelector('#attendance-pop-up .attendance-table') || document.querySelector('.attendance-table');
 if(!table) return;
 const button=table.parentElement.querySelector('#attendance-save');
 const status=table.parentElement.querySelector('#attendance-status');
 const labels=[table.dataset.unmarked,table.dataset.present,table.dataset.absent];
 const cells=[...table.querySelectorAll('td.presence-check')];
 let dirty=false;
 const refresh=cell=>{const state=Number(cell.dataset.state)||0;cell.setAttribute('aria-label',cell.dataset.stamp+' · '+cell.closest('tr').querySelector('td').innerText.trim()+' · '+labels[state]);cell.title=labels[state];};
 cells.forEach(cell=>{
  cell.dataset.initial=cell.dataset.state;cell.tabIndex=0;cell.setAttribute('role','button');refresh(cell);
  const toggle=()=>{cell.dataset.state=(Number(cell.dataset.state)+1)%3;dirty=cells.some(c=>c.dataset.state!==c.dataset.initial);table.dataset.dirty=String(dirty);refresh(cell);};
  cell.onclick=toggle;
  cell.onkeydown=e=>{if(e.key==='Enter'||e.key===' '){e.preventDefault();toggle();}};
 });
 button.onclick=async e=>{
  e.preventDefault();if(!dirty){status.textContent=table.dataset.saved;return;}
  if(!cells.some(cell=>cell.dataset.state!==cell.dataset.initial)){dirty=false;table.dataset.dirty="false";status.textContent=table.dataset.saved;return;}
  button.disabled=true;button.classList.add('spin');status.textContent='';
  try {
   const data=cells.filter(cell=>cell.dataset.state!==cell.dataset.initial).map(cell=>({StudentId:cell.closest('tr').dataset.id,State:cell.dataset.state,Id:cell.dataset.id,Stamp:cell.dataset.stamp}));
   const response=await fetch(button.dataset.action,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(data)});
   if(!response.ok || await response.json()!==true) throw new Error('save');
   dirty=false;table.dataset.dirty="false";status.textContent=table.dataset.saved;
   // Reload persisted IDs before a second save, avoiding duplicate records.
   const link=document.querySelector('#attendance-pop-up .attendance-date a');
   if(link){const url=new URL(link.href);const first=cells[0]?.dataset.stamp;if(first){const [d,m,y]=first.split('/');url.searchParams.set('date',`${y}-${m}-${d}`);const message=table.dataset.saved;$('#attendance-pop-up').load(url.pathname+url.search,function(_,state){const feedback=this.querySelector('#attendance-status');if(feedback)feedback.textContent=state==='error'?table.dataset.error:message;if(state==='error')button.disabled=true;});}}
  } catch {status.textContent=table.dataset.error;}
  finally{button.disabled=false;button.classList.remove('spin');}
 };
})();
