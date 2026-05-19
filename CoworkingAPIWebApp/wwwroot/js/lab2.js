
let activeBookings = {};

async function getPlacesForBooking() {
   
    try {
        const bookingsResponse = await fetch('api/Bookings');
        if (bookingsResponse.ok) {
            const allBookings = await bookingsResponse.json();

            activeBookings = {};

            const formatTime = (isoString) => {
                const d = new Date(isoString);
                return `${String(d.getDate()).padStart(2, '0')}.${String(d.getMonth() + 1).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
            };

            allBookings.forEach(b => {
              
                if (!activeBookings[b.placeId]) activeBookings[b.placeId] = [];

                const timeRangeText = `${formatTime(b.startTime)} - ${formatTime(b.endTime).split(' ')[1]}`;

                activeBookings[b.placeId].push({
                    id: b.id,
                    time: timeRangeText,
                    userId: b.userId 
                });
            });
        }
    } catch (bookingError) {
        console.warn('Не вдалося завантажити історію бронювань:', bookingError);
    }

   
    try {
        const response = await fetch('api/Places');
        const places = await response.json();

        const container = document.getElementById('places-container');
        container.innerHTML = '';

        places.forEach(place => {
            const card = document.createElement('div');
            card.className = 'place-card';

            card.innerHTML = `
                <img src="images/${place.article}.jpg" alt="${place.name}" onerror="this.src='images/default.jpg'">
                <h3>${place.name}</h3>
                <p>Артикул: ${place.article}</p>
                <p>Ціна: <strong>${place.pricePerHour} грн/год</strong></p>
                
                <div id="user-bookings-${place.id}" class="user-bookings-list"></div>

                <button id="btn-place-${place.id}" onclick="bookPlace(${place.id})">Забронювати</button>
            `;
            container.appendChild(card);

           
            if (activeBookings[place.id] && activeBookings[place.id].length > 0) {
                renderUserBookings(place.id);
            }
        });
    } catch (error) {
        console.error('Критична помилка отримання місць:', error);
    }
}

async function bookPlace(id) {
    document.getElementById('bookingModal').style.display = 'flex';
    document.getElementById('placeId').value = id;
    document.getElementById('display-place-id').innerText = id;

    const priceBlock = document.getElementById('total-price-block');
    priceBlock.style.background = '#eef7ff';
    priceBlock.style.border = 'none';
    document.getElementById('total-price-value').innerText = '0';

    const submitBtn = document.querySelector('#bookingForm button[type="submit"]');
    if (submitBtn) {
        submitBtn.disabled = false;
        submitBtn.innerText = 'Підтвердити бронювання';
        submitBtn.style.backgroundColor = '';
    }

    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    document.getElementById('startTime').min = now.toISOString().slice(0, 16);

    document.getElementById('services-container').innerHTML = 'Завантаження послуг...';
    document.getElementById('equipment-container').innerHTML = 'Завантаження обладнання...';

    try {
        const [servicesRes, equipmentRes] = await Promise.all([
            fetch('api/Services'),
            fetch('api/Equipments')
        ]);

        if (servicesRes.ok) {
            const services = await servicesRes.json();
            const sContainer = document.getElementById('services-container');
            sContainer.innerHTML = services.map(s => `
                <div class="checkbox-item">
                    <input type="checkbox" name="selectedServices" value="${s.id}">
                    <label>${s.name} (+${s.price} грн)</label>
                </div>
            `).join('') || 'Немає доступних послуг';
        }

        if (equipmentRes.ok) {
            const equipment = await equipmentRes.json();
            const eContainer = document.getElementById('equipment-container');
            eContainer.innerHTML = equipment.map(e => `
                <div class="checkbox-item">
                    <input type="checkbox" name="selectedEquipment" value="${e.id}">
                    <label>${e.name} (+${e.pricePerHour} грн/год)</label>
                </div>
            `).join('') || 'Немає додаткового обладнання';
        }

    } catch (err) {
        console.error('Помилка завантаження додаткових опцій:', err);
    }
}

function renderUserBookings(placeId) {
    const listContainer = document.getElementById(`user-bookings-${placeId}`);
    if (!listContainer) return;

    const bookings = activeBookings[placeId] || [];

    if (bookings.length === 0) {
        listContainer.innerHTML = '';
        return;
    }

    listContainer.innerHTML = `
        <div style="font-weight: bold; margin-bottom: 6px; color: #555; text-align: left;">Зайнятий час:</div>
        ` + bookings.map(b => {
        const currentUserId = 2; 

        if (b.userId === currentUserId) {
           
            return `
                    <div class="user-booking-item" style="border-left-color: #28a745;">
                        <span>${b.time} <strong>(Ви)</strong></span>
                        <button class="cancel-booking-btn" onclick="cancelBooking(${placeId}, ${b.id})">✕</button>
                    </div>
                `;
        } else {
            
            return `
                    <div class="user-booking-item" style="border-left-color: #ffc107; background: #fffcf5; color: #666;">
                        <span>${b.time} <em>(Зайнято)</em></span>
                        <span style="font-size: 0.85em; color: #aaa; padding-right: 4px;">🔒</span>
                    </div>
                `;
        }
    }).join('');
}
async function cancelBooking(placeId, bookingId) {
    if (!confirm("Ви дійсно хочете скасувати це бронювання?")) {
        return;
    }

    try {
        const response = await fetch(`api/Bookings/${bookingId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Бронювання успішно скасовано!");

           
            activeBookings[placeId] = activeBookings[placeId].filter(b => b.id !== bookingId);

            
            renderUserBookings(placeId);
        } else {
            const errText = await response.text();
            alert(`Не вдалося скасувати: ${errText}`);
        }
    } catch (err) {
        console.error('Помилка при скасуванні:', err);
        alert('Сталася помилка при з’єднанні з сервером.');
    }
}

function showErrorMessage(message) {
    const errorDiv = document.getElementById('error-summary');
    errorDiv.innerText = message;
    errorDiv.style.display = 'block';
}

function closeModal() {
    document.getElementById('bookingModal').style.display = 'none';
    document.getElementById('bookingForm').reset();
    document.getElementById('error-summary').style.display = 'none';
}

document.getElementById('bookingForm').onsubmit = async (e) => {
    e.preventDefault();

    const participants = parseInt(document.getElementById('participantsCount').value);
    document.getElementById('error-summary').style.display = 'none';

    const startTimeVal = document.getElementById('startTime').value;
    const endTimeVal = document.getElementById('endTime').value;
    const start = new Date(startTimeVal);
    const end = new Date(endTimeVal);
    const now = new Date();

    if (!startTimeVal || !endTimeVal) {
        showErrorMessage("Будь ласка, оберіть час початку та завершення!");
        return;
    }

    if (start < now) {
        showErrorMessage("Не можна створювати бронювання на минулий час!");
        return;
    }

    if (end <= start) {
        showErrorMessage("Час завершення має бути пізніше часу початку!");
        return;
    }

    const durationMs = end - start;
    if (durationMs < 30 * 60 * 1000) {
        showErrorMessage("Мінімальний час бронювання — 30 хвилин.");
        return;
    }

    if (participants < 1 || participants > 200) {
        showErrorMessage("Кількість учасників повинна бути від 1 до 200!");
        return;
    }

    const selectedServices = Array.from(document.querySelectorAll('input[name="selectedServices"]:checked'))
        .map(cb => ({ id: parseInt(cb.value) }));

    const selectedEquipment = Array.from(document.querySelectorAll('input[name="selectedEquipment"]:checked'))
        .map(cb => ({ id: parseInt(cb.value) }));

    const bookingData = {
        placeId: parseInt(document.getElementById('placeId').value),
        startTime: startTimeVal,
        endTime: endTimeVal,
        participantsCount: participants,
        notes: document.getElementById('notes').value,
        userId: 2, 
        statusId: 1,
        services: selectedServices,
        equipment: selectedEquipment
    };

    try {
        const submitBtn = e.target.querySelector('button[type="submit"]');
        submitBtn.disabled = true;
        submitBtn.innerText = 'Обробка...';

        const response = await fetch('api/Bookings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bookingData)
        });

        if (response.ok) {
            const createdBooking = await response.json();
            const finalPrice = createdBooking.totalPrice || createdBooking.price || 0;
            const currentPlaceId = parseInt(document.getElementById('placeId').value);

            const formatTime = (isoString) => {
                const d = new Date(isoString);
                return `${String(d.getDate()).padStart(2, '0')}.${String(d.getMonth() + 1).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
            };
            const timeRangeText = `${formatTime(startTimeVal)} - ${formatTime(endTimeVal).split(' ')[1]}`;

            if (!activeBookings[currentPlaceId]) activeBookings[currentPlaceId] = [];

            
            activeBookings[currentPlaceId].push({
                id: createdBooking.id,
                time: timeRangeText,
                userId: 1
            });

            document.getElementById('total-price-value').innerText = finalPrice.toFixed(2);

            const priceBlock = document.getElementById('total-price-block');
            priceBlock.style.background = '#e6fced';
            priceBlock.style.border = '1px solid #28a745';

           
            renderUserBookings(currentPlaceId);

            submitBtn.innerText = 'Успішно заброньовано!';
            submitBtn.style.backgroundColor = '#28a745';

        } else {
            submitBtn.disabled = false;
            submitBtn.innerText = 'Підтвердити бронювання';
            const errorText = await response.text();
            showErrorMessage(errorText);
        }
    } catch (err) {
        console.error('Помилка при відправці:', err);
        showErrorMessage('Не вдалося з’єднатися з сервером.');
        const submitBtn = e.target.querySelector('button[type="submit"]');
        submitBtn.disabled = false;
        submitBtn.innerText = 'Підтвердити бронювання';
    }
};