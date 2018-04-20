import Vue from 'vue';
import Locales from './locale';
import zhLocale from 'iview/src/locale/lang/zh-CN';
import enLocale from 'iview/src/locale/lang/en-US';
import VueI18n from 'vue-i18n';

Vue.use(VueI18n);
export default new VueI18n({
    locale: 'zh-CN',
    fallbackLocale: 'en-US',
    messages:Locales
  });