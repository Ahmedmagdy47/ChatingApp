import { Service } from '@angular/core';

@Service()
export class ToastService {
    constructor() {
        this.createToastContainer();
    }

    private createToastContainer() {
        if (!document.getElementById('toast-container')) {
            const container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast toast-bottom toast-end z-50';
            document.body.appendChild(container);
        }
    }

    private createToastElement(message: string, alertClass: string, duration: number) {
        const toastContainer = document.getElementById('toast-container');
        if (!toastContainer) return;

        const toast = document.createElement('div');
        toast.classList.add('alert', alertClass, 'shadow-lg');
        toast.innerHTML = `
            <span>${message}</span>
            <button class="ml-4 btn btn-sm btn-ghost">x</button>
        `

        toast.querySelector('button')?.addEventListener('click', () => {
            toastContainer.removeChild(toast);
        })

        toastContainer.append(toast);

        setTimeout(() => {
            if (toastContainer.contains(toast)) {
                toastContainer.removeChild(toast);
            }
        }, duration);
    }

    Success(message: string, duration?: number) {
        this.createToastElement(message, 'alert-success', duration || 5000);
    }
    Error(message: string, duration?: number) {
        this.createToastElement(message, 'alert-error', duration || 5000);
    }
    Warning(message: string, duration?: number) {
        this.createToastElement(message, 'alert-warning', duration || 5000);
    }
    Info(message: string, duration?: number) {
        this.createToastElement(message, 'alert-info', duration || 5000);
    }
}
