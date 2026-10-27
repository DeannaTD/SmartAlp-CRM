/* Presentation only: existing form submission and AJAX behavior stay in their original scripts. */
(() => {
    'use strict';
    document.addEventListener('DOMContentLoaded', () => {
        const nav = document.querySelector('nav.primary-nav');
        const toggle = document.querySelector('.nav-toggler');
        const mobile = document.querySelector('.mobile-menu-toggle');
        const content = document.querySelector('main .content');
        if (!nav || !content) return;
        const narrow = window.matchMedia('(max-width: 760px)');
        const path = location.pathname.toLowerCase().replace(/\/$/, '') || '/';
        let selected;
        nav.querySelectorAll('a[href]').forEach(link => {
            if (link.getAttribute('href').startsWith('#')) return;
            const url = new URL(link.href, location.href);
            if (!url.hash && (url.pathname.toLowerCase().replace(/\/$/, '') || '/') === path) {
                link.setAttribute('aria-current', 'page');
                selected = link;
                link.closest('.has-dropdown')?.classList.add('open');
            }
        });
        // Detail pages retain their parent section's context.
        if (!selected) {
            const parents = [
                [/^\/level\//, nav.querySelector('a[href="/Level/MentorIndex"]') ? '/Level/MentorIndex' : nav.querySelector('a[href="/Level/StudentProgress"]') ? '/Level/StudentProgress' : '/Level/LevelControlMenu'],
                [/^\/branch(?:\/index)?$/, '/'],
                [/^\/account\/(student|archivedstudent|newstudent)/, '/Account/Students'],
                [/^\/account\/mentor/, '/Account/Mentors'],
                [/^\/hub\//, '/Hub'],
                [/^\/contract\/(bill|royalty)/, path.includes('royalty') ? '/Contract/RoyaltyTable' : '/Contract/BillTable'],
                [/^\/contract\//, '/Contract/ContractTable']
            ];
            for (const [match, target] of parents) {
                if (!match.test(path)) continue;
                selected = [...nav.querySelectorAll('a[href]')].find(a => new URL(a.href).pathname.toLowerCase() === target.toLowerCase());
                if (selected) { selected.setAttribute('aria-current', 'page'); selected.closest('.has-dropdown')?.classList.add('open'); }
                break;
            }
        }
        const locationLabel = document.querySelector('[data-current-section]');
        const heading = content.querySelector('h1,h2');
        if (locationLabel && (selected || heading)) locationLabel.textContent = selected?.textContent.trim() || heading.textContent.trim();

        const syncNavigation = () => {
            toggle?.setAttribute('aria-expanded', String(!nav.classList.contains('hidden')));
            nav.querySelectorAll('.dropdown-toggle').forEach(a => a.setAttribute('aria-expanded', String(a.closest('.has-dropdown').classList.contains('open'))));
        };
        try { if (!narrow.matches && localStorage.getItem('smartalp-sidebar-collapsed') === 'true') nav.classList.add('hidden'); } catch { /* Storage is optional. */ }
        toggle?.addEventListener('click', () => setTimeout(() => {
            syncNavigation();
            try { localStorage.setItem('smartalp-sidebar-collapsed', String(nav.classList.contains('hidden'))); } catch { }
        }, 0));
        nav.querySelectorAll('.dropdown-toggle').forEach(a => a.addEventListener('click', () => {
            nav.classList.remove('hidden');
            setTimeout(syncNavigation, 0);
        }));
        const closeMobile = () => { document.body.classList.remove('nav-open'); mobile?.setAttribute('aria-expanded', 'false'); };
        mobile?.addEventListener('click', () => {
            nav.classList.remove('hidden');
            const open = document.body.classList.toggle('nav-open');
            mobile.setAttribute('aria-expanded', String(open));
        });
        narrow.addEventListener('change', () => { closeMobile(); if (narrow.matches) nav.classList.remove('hidden'); syncNavigation(); });
        if (narrow.matches) nav.classList.remove('hidden');
        document.querySelector('main')?.addEventListener('click', e => { if (!e.target.closest('.mobile-menu-toggle')) closeMobile(); });
        syncNavigation();

        document.querySelectorAll('[data-schedule-day]').forEach(button => {
            button.addEventListener('click', () => {
                const day = button.dataset.scheduleDay;
                document.querySelectorAll('[data-schedule-day]').forEach(item => item.setAttribute('aria-pressed', String(item === button)));
                const rows = [...document.querySelectorAll('.schedule-row')];
                rows.forEach(row => { row.hidden = day !== 'all' && row.dataset.day !== day; });
                document.querySelectorAll('.schedule-branch').forEach(branch => { branch.hidden = !branch.querySelector('.schedule-row:not([hidden])'); });
                const empty = document.getElementById('schedule-empty');
                if (empty) empty.hidden = rows.some(row => !row.hidden);
            });
        });

        document.querySelector('[data-schedule-initial="true"]')?.click();

        const enhancedDialogs = new WeakSet();
        const enhanceContent = () => {
            content.querySelectorAll('table.table, table.access-table, table.attendance-table').forEach(table => {
                if (table.closest('.table-scroll')) return;
                const wrapper = document.createElement('div');
                wrapper.className = 'table-scroll';
                wrapper.tabIndex = 0;
                wrapper.setAttribute('role', 'region');
                wrapper.setAttribute('aria-label', heading?.textContent.trim() || 'Data table');
                table.before(wrapper);
                wrapper.append(table);
            });
            content.querySelectorAll('.table-scroll input:not([type=hidden]), .table-scroll select').forEach(input => {
                if (input.hasAttribute('aria-label') || input.labels?.length) return;
                const cell = input.closest('td');
                const table = cell?.closest('table');
                const header = table?.rows[0]?.cells[cell.cellIndex]?.textContent.trim();
                const row = cell?.parentElement;
                if (header) input.setAttribute('aria-label', `${header} · ${row.cells[0]?.textContent.trim() || ''}`);
            });
            content.querySelectorAll('[data-link]:not(a):not(button)').forEach(item => {
                if (item.dataset.keyboardReady) return;
                item.dataset.keyboardReady = 'true';
                item.tabIndex = 0;
                item.setAttribute('role', 'link');
                const label = item.querySelector('.file-control-title')?.textContent.trim();
                if (label) item.setAttribute('aria-label', label);
                item.addEventListener('keydown', e => {
                    if (e.target === item && e.key === 'Enter') { e.preventDefault(); item.click(); }
                });
            });
            content.querySelectorAll('.pop-up-wrapper').forEach(dialog => {
                if (enhancedDialogs.has(dialog)) return;
                enhancedDialogs.add(dialog);
                dialog.setAttribute('role', 'dialog');
                dialog.setAttribute('aria-modal', 'true');
                dialog.setAttribute('aria-label', dialog.querySelector('h2,h3')?.textContent.trim() || locationLabel?.textContent || 'Form');
                dialog.querySelector('.close-icon')?.setAttribute('aria-label', 'Close');
                const before = document.activeElement;
                dialog.addEventListener('keydown', e => {
                    if (e.key !== 'Tab') return;
                    const controls = [...dialog.querySelectorAll('button:not(:disabled),a[href],input:not([type=hidden]):not(:disabled),select,textarea,[tabindex="0"]')].filter(el => el.getClientRects().length);
                    const first = controls[0], last = controls[controls.length - 1];
                    if (e.shiftKey && document.activeElement === first) { e.preventDefault(); last?.focus(); }
                    else if (!e.shiftKey && document.activeElement === last) { e.preventDefault(); first?.focus(); }
                });
                dialog.querySelector('.close-icon')?.addEventListener('click', () => { if (before?.isConnected) before.focus(); });
                dialog.querySelector('input:not([type=hidden]),select,textarea,button')?.focus();
            });
        };
        enhanceContent();
        new MutationObserver(enhanceContent).observe(content, { childList: true, subtree: true });
        document.addEventListener('keydown', e => {
            if (e.key !== 'Escape') return;
            const dialog = content.querySelector('.pop-up-wrapper');
            if (dialog) dialog.querySelector('.close-icon')?.click();
            else if (document.body.classList.contains('nav-open')) { closeMobile(); mobile?.focus(); }
        });
    });
})();

document.addEventListener('click', function(event) {
 const link=event.target.closest('a[href]'); if(!link) return;
 const url=new URL(link.href,location.href);
 if(!/\/(RemoveAccess|SwitchUserStatus|ToggleUserStatus|ArchiveStudent)$/.test(url.pathname)) return;
 const ru=document.documentElement.lang.startsWith('ru') || document.cookie.includes('lang=ru');
 const row=link.closest('tr');
 const subject=row ? row.innerText.trim().split('\n').slice(0,3).join(' · ') : '';
 if(!window.confirm((ru?'Подтвердите действие: ':'Confirm action: ')+(link.title || link.textContent.trim())+'\n'+subject)) {
  event.preventDefault(); event.stopImmediatePropagation();
 }
},true);

document.addEventListener('click',function(e){
 const link=e.target.closest('a[href*="SwitchUserStatus"]');if(!link)return;
 const url=new URL(link.href);const group=link.closest('tr')?.querySelector('select[name="groupId"]');
 if(url.searchParams.get('statusId')==='1' && group && group.value==='-1'){
  e.preventDefault();e.stopImmediatePropagation();group.focus();
  alert(document.cookie.includes('lang=ru')?'Выберите группу и нажмите «Перевести», чтобы восстановить ученика.':'Choose a group and click Move to restore this student.');
 }
},true);

// Closing or changing the journal week must not silently discard edits.
document.addEventListener('click',function(e){
 const table=document.querySelector('#attendance-pop-up .attendance-table[data-dirty="true"]');
 if(!table)return;
 const target=e.target.closest('#attendance-pop-up .close-icon, #attendance-pop-up a[data-ajax-update="#attendance-pop-up"]');
 if(!target)return;
 const message=document.cookie.includes('lang=ru')?'Есть несохранённые отметки. Выйти без сохранения?':'There are unsaved attendance marks. Leave without saving?';
 if(!confirm(message)){e.preventDefault();e.stopImmediatePropagation();}
},true);
window.addEventListener('beforeunload',function(e){
 if(document.querySelector('#attendance-pop-up .attendance-table[data-dirty="true"]')){e.preventDefault();e.returnValue='';}
});
