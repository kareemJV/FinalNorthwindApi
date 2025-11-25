let allProducts = [];

async function fetchProducts() {
    try {
    const response = await fetch('http://localhost:5299/api/products');
    const products = await response.json();
    console.log(products);
    allProducts = products;
    return products;
}catch (error) {
    console.error('Error fetching products:', error);
}
}

function renderProducts(products) {
    const tableBody = document.getElementById('product-table-body');
    tableBody.innerHTML = '';

    products.forEach(product => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${product.productID}</td>
            <td>${product.productName}</td>
            <td>${product.categoryName}</td>
            <td>${product.SupplierName}</td>
            <td>${product.unitPrice} €</td>
            <td>${product.unitsInStock}</td>
            <td><button class="delete-btn" data-id="${product.productID}">Delete</button></td>
        `;
        tableBody.appendChild(row);
    });

    // Add event listeners for delete buttons
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            const id = btn.getAttribute('data-id');
            if (confirm('Produkt wirklich löschen?')) {
                try {
                    const response = await fetch(`http://localhost:5299/api/products/${id}`, {
                        method: 'DELETE'
                    });
                    if (response.ok) {
                        // Remove from allProducts and re-render
                        allProducts = allProducts.filter(p => p.productID != id);
                        renderProducts(allProducts);
                    } else {
                        alert('Löschen fehlgeschlagen!');
                    }
                } catch (error) {
                    alert('Fehler beim Löschen!');
                }
            }
        });
    });
}

function handleSearchInput(event) {
    const searchTerm = event.target.value.toLowerCase();
    const filtered = allProducts.filter(product =>
        product.productName.toLowerCase().includes(searchTerm)
    );
    renderProducts(filtered);
}


fetchProducts().then(products => {
    renderProducts(products);
});

document.getElementById('search-input').addEventListener('input', handleSearchInput);

document.getElementById('add-product-button').addEventListener('click', () => {
    window.location.href = 'addProduct.html';
});

