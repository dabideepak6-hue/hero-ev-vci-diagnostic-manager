const products=[
{id:1,name:"Wireless Bluetooth Earbuds",category:"Electronics",price:"Check current price",rating:"★★★★★",image:"https://placehold.co/600x500?text=Earbuds",url:"https://www.amazon.in/"},
{id:2,name:"Smart Watch",category:"Electronics",price:"Check current price",rating:"★★★★☆",image:"https://placehold.co/600x500?text=Smart+Watch",url:"https://www.amazon.in/"},
{id:3,name:"Car Mobile Holder",category:"Automotive",price:"Check current price",rating:"★★★★★",image:"https://placehold.co/600x500?text=Car+Holder",url:"https://www.amazon.in/"},
{id:4,name:"LED Desk Lamp",category:"Home",price:"Check current price",rating:"★★★★☆",image:"https://placehold.co/600x500?text=LED+Lamp",url:"https://www.amazon.in/"},
{id:5,name:"Gaming Mouse",category:"Gaming",price:"Check current price",rating:"★★★★★",image:"https://placehold.co/600x500?text=Gaming+Mouse",url:"https://www.amazon.in/"},
{id:6,name:"Professional Tool Kit",category:"Tools",price:"Check current price",rating:"★★★★☆",image:"https://placehold.co/600x500?text=Tool+Kit",url:"https://www.amazon.in/"}
];
let activeCategory="All";
function categories(){return ["All",...new Set(products.map(p=>p.category))]}
function renderCategories(){document.getElementById("categoryBar").innerHTML=categories().map(c=>`<button class="chip ${c===activeCategory?"active":""}" onclick="setCategory('${c}')">${c}</button>`).join("")}
function setCategory(c){activeCategory=c;renderCategories();renderProducts()}
function renderProducts(){
 const q=document.getElementById("search").value.toLowerCase();
 const list=products.filter(p=>(activeCategory==="All"||p.category===activeCategory)&&(p.name+" "+p.category).toLowerCase().includes(q));
 document.getElementById("count").textContent=`${list.length} products`;
 document.getElementById("products").innerHTML=list.length?list.map(p=>`<article class="card"><img src="${p.image}" alt="${p.name}" loading="lazy"><div class="card-body"><span class="tag">${p.category}</span><h3>${p.name}</h3><div class="rating">${p.rating}</div><div class="price">${p.price}</div><p>Product details and current availability are shown by the merchant. Verify final price before purchase.</p><div class="actions"><a class="buy" href="${p.url}" target="_blank" rel="nofollow sponsored noopener">Check Best Deal</a><button class="share" onclick="shareProduct('${p.name}')">Share</button></div></div></article>`).join(""):`<div class="empty"><h3>No products found</h3><p>Try another search or category.</p></div>`;
}
function shareProduct(name){const text=`Check this product: ${name} — Deepak Dabi Deals`;if(navigator.share)navigator.share({title:name,text,url:location.href});else navigator.clipboard?.writeText(text+" "+location.href).then(()=>alert("Link copied!"))}
function toggleMenu(){document.getElementById("nav").classList.toggle("open")}
renderCategories();renderProducts();
if("serviceWorker"in navigator)window.addEventListener("load",()=>navigator.serviceWorker.register("sw.js"));
